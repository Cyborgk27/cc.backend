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
    private readonly IPasswordHasher _hasher;
    private readonly IEmailService _emailService;
    private readonly IUserContext _userContext; // Inyectamos seguridad

    public UserApplication(
        IUnitOfWork unitOfWork,
        ServiceData serviceData,
        IPasswordHasher hasher,
        IEmailService emailService,
        IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _serviceData = serviceData;
        _hasher = hasher;
        _emailService = emailService;
        _userContext = userContext;
    }

    public async Task<BaseResponse<bool>> SaveUserAsync(RegisterRequest dto)
    {
        // SEGURIDAD: Solo Admin puede crear o editar usuarios directamente
        if (!_userContext.IsInRole("ADMINISTRATOR"))
            throw new UnauthorizedAccessException("No tienes permiso para gestionar usuarios.");

        User user;

        if (dto.Id.HasValue && dto.Id != Guid.Empty)
        {
            user = await _unitOfWork.Users.GetByIdAsync(dto.Id.Value);
            if (user == null || user.IsDeleted) throw new EntityNotFoundException("User", dto.Id.Value);

            user.UpdateProfile(dto.FirstName, dto.LastName, null);
            user.AssignRole(dto.RoleId);

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var newHash = _hasher.Hash(dto.Password);
                // Necesitarás agregar un método public void UpdatePassword(string hash) en tu entidad User
                typeof(User).GetProperty("PasswordHash")?.SetValue(user, newHash);
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new DomainException("PASSWORD_REQUIRED", "Contraseña requerida", "La contraseña es obligatoria.");

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
        // SEGURIDAD: Solo Admin puede listar usuarios
        if (!_userContext.IsInRole("ADMINISTRATOR"))
            throw new UnauthorizedAccessException("Acceso denegado a la lista de usuarios.");

        var pagedResult = await _unitOfWork.Users.GetPagedAsync(
            page,
            size,
            filter: x => (string.IsNullOrEmpty(search) ||
                         x.Email.Contains(search!) ||
                         x.UserName.Contains(search!) ||
                         x.FirstName.Contains(search!)),
            orderBy: x => x.OrderBy(f => f.AuditCreateDate),
            includeProperties: "Role"
        );

        var dtos = pagedResult.Items.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            Password = null,
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
        // SEGURIDAD: Un usuario puede verse a sí mismo, o ser visto por un Admin
        if (!_userContext.IsInRole("ADMINISTRATOR") && _userContext.UserId != id)
            throw new UnauthorizedAccessException("No puedes consultar la información de otro usuario.");

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
        // SEGURIDAD: Solo Admin
        if (!_userContext.IsInRole("ADMINISTRATOR"))
            throw new UnauthorizedAccessException("Solo un administrador puede aprobar cuentas.");

        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return _serviceData.CreateResponse(false, "Usuario no encontrado.", 404);

        try
        {
            user.Activate();
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            await _emailService.SendNotificationActiveAccount(user.Email, user.UserName);

            return _serviceData.CreateResponse(true, "Usuario activado y aprobado exitosamente.");
        }
        catch (DomainException ex)
        {
            return _serviceData.CreateResponse(false, ex.Message, 400);
        }
    }

    public async Task<BaseResponse<bool>> DeactivateUserAsync(Guid userId)
    {
        // SEGURIDAD: Solo Admin
        if (!_userContext.IsInRole("ADMINISTRATOR"))
            throw new UnauthorizedAccessException("Solo un administrador puede desactivar cuentas.");

        var user = await _unitOfWork.Users.GetUserWithRoleAsync(userId);

        if (user == null) return _serviceData.CreateResponse(false, "Usuario no encontrado.", 404);

        if(user.Role != null && user.Role.Name == "ADMINISTRATOR")
            return _serviceData.CreateResponse(false, "No se puede desactivar un usuario con rol de administrador.", 403);

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
    }
}