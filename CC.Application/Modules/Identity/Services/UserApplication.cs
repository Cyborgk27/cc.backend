using CC.Application.Common.Bases;
using CC.Application.Common.Helpers;
using CC.Application.Common.Interfaces;
using CC.Application.Modules.Identity.Dtos;
using CC.Application.Modules.Identity.Interfaces;
using CC.Domain.Entities;
using CC.Domain.Exceptions;
using CC.Domain.Repositories;
using CC.Utilities.Static;

public class UserApplication : IUserApplication
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ServiceData _serviceData;
    private readonly IPasswordHasher _hasher; // Tu servicio de hashing inyectado
    private readonly IEmailService _emailService; // Para enviar correos

    public UserApplication(IUnitOfWork unitOfWork, ServiceData serviceData, IPasswordHasher hasher, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _serviceData = serviceData;
        _hasher = hasher;
        _emailService = emailService;
    }

    public async Task<BaseResponse<bool>> SaveUserAsync(RegisterRequest dto)
    {
        User user;

        if (dto.Id.HasValue && dto.Id != Guid.Empty)
        {
            // --- MODO EDICIÓN ---
            user = await _unitOfWork.Users.GetByIdAsync(dto.Id.Value);
            if (user == null || user.IsDeleted) throw new EntityNotFoundException("User", dto.Id.Value);

            // Actualizamos perfil y rol
            user.UpdateProfile(dto.FirstName, dto.LastName, null); // PhoneNumber null por ahora
            user.AssignRole(dto.RoleId);

            // Solo actualizamos password si el usuario envió uno nuevo
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var newHash = _hasher.Hash(dto.Password);
                // Necesitarás agregar un método public void UpdatePassword(string hash) en tu entidad User
                typeof(User).GetProperty("PasswordHash")?.SetValue(user, newHash);
            }
        }
        else
        {
            // --- MODO CREACIÓN ---
            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new DomainException("PASSWORD_REQUIRED", "Contraseña requerida", "La contraseña es obligatoria para nuevos usuarios.");

            var passwordHash = _hasher.Hash(dto.Password);

            user = new User(
                dto.Email,
                dto.UserName,
                dto.FirstName,
                dto.LastName,
                passwordHash,
                dto.RoleId
            );


            await _unitOfWork.Users.AddAsync(user);
        }

        await _unitOfWork.SaveChangesAsync();
        return _serviceData.CreateResponse(true, ReplyMessage.MESSAGE_SAVE);
    }

    public async Task<BaseResponse<IEnumerable<UserDto>>> GetPagedUsersAsync(int page, int size, string? search = null)
    {
        var pagedResult = await _unitOfWork.Users.GetPagedAsync(
            page,
            size,
            filter: x => (string.IsNullOrEmpty(search) ||
                         x.Email.Contains(search) ||
                         x.UserName.Contains(search) ||
                         x.FirstName.Contains(search)),
            orderBy: x => x.OrderBy(f => f.AuditCreateDate),
            includeProperties: "Role"
        );

        var dtos = pagedResult.Items.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            Password = null, // Jamás devolvemos el hash
            FirstName = u.FirstName,
            LastName = u.LastName,
            RoleId = u.RoleId,
            RoleName = u.Role?.ShowName,
            IsDeleted = u.IsDeleted
        });

        return _serviceData.CreateResponse(dtos, ReplyMessage.MESSAGE_QUERY, 200, pagedResult.TotalCount);
    }

    public async Task<BaseResponse<UserDto>> GetUserByIdAsync(Guid id)
    {
        // Usamos el método que creamos en el repositorio para traer el Include(Role)
        var user = await _unitOfWork.Users.GetUserWithRoleAsync(id);

        if (user == null || user.IsDeleted) throw new EntityNotFoundException("User", id);

        var dto = new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Password = null,
            FirstName = user.FirstName,
            LastName = user.LastName,
            RoleId = user.RoleId,
            RoleName = user.Role?.Name,
            IsDeleted = user.IsDeleted
        };

        return _serviceData.CreateResponse(dto, ReplyMessage.MESSAGE_QUERY);
    }

    public async Task<BaseResponse<bool>> ActivateUserAsync(Guid userId)
    {
        // 1. Buscamos al usuario por su ID
        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        if (user == null)
        {
            return _serviceData.CreateResponse(false, "Usuario no encontrado.", 404);
        }

        try
        {
            // 2. Ejecutamos la regla de negocio definida en la Entidad de Dominio
            user.Activate();

            // 3. Actualizamos y guardamos
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            await _emailService.SendNotificationActiveAccount(user.Email, user.UserName);

            return _serviceData.CreateResponse(true, "Usuario activado y aprobado exitosamente.");
        }
        catch (DomainException ex)
        {
            // Capturamos si la entidad lanza una excepción (ej: ya estaba activo)
            return _serviceData.CreateResponse(false, ex.Message, 400);
        }
        catch (Exception ex)
        {
            return _serviceData.CreateResponse(false, "Error interno al activar usuario.", 500);
        }
    }

    public async Task<BaseResponse<bool>> DeactivateUserAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        if(user == null)
        {
            return _serviceData.CreateResponse(false, "Usuario no encontrado.", 404);
        }

        try
        {
            user.Deactivate();
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return _serviceData.CreateResponse(true, "Usuario desactivado exitosamente.");
        }
        catch (DomainException ex)
        {
            return _serviceData.CreateResponse(false, ex.Message, 400);
        }
        catch (Exception ex)
        {
            return _serviceData.CreateResponse(false, "Error interno al desactivar usuario.", 500);
        }
    }
}