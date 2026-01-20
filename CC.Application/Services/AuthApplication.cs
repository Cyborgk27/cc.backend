using CC.Application.Common.Bases;
using CC.Application.Common.Helpers;
using CC.Application.DTOs.Auth;
using CC.Application.DTOs.User;
using CC.Application.Interfaces;
using CC.Domain.Entities;
using CC.Domain.Exceptions;
using CC.Domain.Repositories;
using CC.Utilities.Static;

namespace CC.Application.Services
{
    public class AuthApplication : IAuthApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ServiceData _serviceData;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IPasswordHasher _hasher;
        private readonly IEmailService _emailService;

        public AuthApplication(
            IUnitOfWork unitOfWork,
            ServiceData serviceData,
            IJwtGenerator jwtGenerator,
            IPasswordHasher hasher,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _serviceData = serviceData;
            _jwtGenerator = jwtGenerator;
            _hasher = hasher;
            _emailService = emailService;
        }

        public async Task<BaseResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var identifier = request.Email.ToLower().Trim();
            var user = (await _unitOfWork.Users.GetAsync(
                filter: u => (u.Email == identifier || u.UserName == identifier) && !u.IsDeleted,
                includeProperties: "Role.RolePermissions.Permission.Feature"
            )).FirstOrDefault();

            if (user == null || !_hasher.Verify(request.Password, user.PasswordHash))
                throw new DomainException("AUTH_FAILED", "Credenciales Inválidas", "Usuario o contraseña incorrectos.");

            // REGLA DE NEGOCIO: Bloquear acceso si no está activo (aprobado por admin)
            if (!user.IsDeleted)
            {
                return _serviceData.CreateResponse<AuthResponse>(
                    null!,
                    "Tu cuenta está pendiente de aprobación por un administrador.",
                    403 // Forbidden
                );
            }

            user.RegisterLogin();
            await _unitOfWork.Users.UpdateAsync(user);

            return await GenerateAuthResponse(user);
        }

        public async Task<BaseResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var user = (await _unitOfWork.Users.GetAsync(
                filter: u => u.RefreshToken == request.RefreshToken && !u.IsDeleted,
                includeProperties: "Role.RolePermissions.Permission"
            )).FirstOrDefault();

            if (user == null || !user.IsRefreshTokenValid(request.RefreshToken))
                throw new DomainException("INVALID_TOKEN", "Sesión Expirada", "Sesión inválida o expirada.");

            return await GenerateAuthResponse(user);
        }

        private async Task<BaseResponse<AuthResponse>> GenerateAuthResponse(User user)
        {
            // 1. Obtener nombres de permisos para el JWT
            var permissions = user.Role.RolePermissions
                .Select(rp => rp.Permission.Name)
                .ToList();

            // 2. Extraer navegación dinámica desde las Features vinculadas a los permisos
            // Usamos Name y ShowName de la entidad Feature
            var navigation = user.Role.RolePermissions
                .Where(rp => rp.Permission.Feature != null && !rp.Permission.Feature.IsDeleted)
                .Select(rp => rp.Permission.Feature!)
                .GroupBy(f => f.Id) // Evitamos duplicados si varios permisos apuntan a la misma Feature
                .Select(g => g.First())
                .Select(f => new NavigationDto(
                    f.Name,      // Identificador técnico
                    f.ShowName,  // Texto para el menú
                    f.Path,      // Ruta de navegación (ej: /projects)
                    f.Icon       // Icono para el UI (ej: pi pi-home)
                ))
                .ToList();

            var rolesList = new List<string> { user.Role.Name };

            // 3. Generar JWT
            var token = _jwtGenerator.GenerateToken(
                user.Id.ToString(),
                user.Email,
                rolesList,
                permissions
            );

            var newRefreshToken = Guid.NewGuid().ToString("N");

            // 4. Actualizar estado del usuario
            user.UpdateRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // 5. Crear DTO de respuesta con la navegación incluida
            var authResponse = new AuthResponse(
                token,
                newRefreshToken,
                user.Email,
                user.UserName,
                user.ShowName, // Tu propiedad de perfil
                rolesList,
                permissions,
                navigation    // <--- El menú dinámico de base de datos
            );

            return _serviceData.CreateResponse(authResponse, "Sesión iniciada correctamente.");
        }

        public async Task<BaseResponse<Guid>> RegisterAsync(UserDto request)
        {
            // ... (Validaciones de existencia y rol se mantienen igual) ...

            string passwordHash = _hasher.Hash(request.Password);

            var newUser = new User(
                request.Email,
                request.UserName,
                request.FirstName,
                request.LastName,
                passwordHash,
                request.RoleId
            );

            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.SaveChangesAsync();

            // 6. Envío de correo informativo
            _ = _emailService.SendConfirmationEmailAsync(
                newUser.Email,
                newUser.FirstName,
                "Tu registro se ha completado. Por favor, espera a que un administrador apruebe tu cuenta."
            );

            return _serviceData.CreateResponse(
                newUser.Id,
                "Registro exitoso. Tu cuenta está en proceso de revisión. Se te notificará por correo una vez que el administrador la apruebe.",
                201 // Created
            );
        }

        public async Task<BaseResponse<bool>> ConfirmEmailAsync(string email, string token)
        {
            // 1. Buscar al usuario
            var user = (await _unitOfWork.Users.GetAsync(
                filter: u => u.Email == email.ToLower().Trim() && !u.IsDeleted
            )).FirstOrDefault();

            if (user == null)
                throw new DomainException("USER_NOT_FOUND", "Usuario no encontrado", "El correo proporcionado no pertenece a ningún usuario activo.");

            // 2. Ejecutar la lógica de confirmación de la Entidad
            // Esto disparará la excepción si el token no coincide o ya fue confirmado
            user.ConfirmEmail(token);

            // 3. Persistir el cambio (EmailConfirmed = true y Token = null)
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return _serviceData.CreateResponse(true, "Correo confirmado exitosamente. Ya puedes iniciar sesión.");
        }
    }
}