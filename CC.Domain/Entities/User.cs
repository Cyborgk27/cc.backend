using CC.Domain.Common;
using CC.Domain.Exceptions;

namespace CC.Domain.Entities
{
    public class User : BaseEntity<Guid>
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string UserName { get; private set; } // Tu 'name'
        public string ShowName => $"{FirstName} {LastName}";

        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string? PhoneNumber { get; private set; }
        public bool EmailConfirmed { get; private set; }
        public string? EmailConfirmationToken { get; private set; }

        // Relación con Rol
        public Guid RoleId { get; private set; }
        public virtual Role Role { get; private set; } = null!;

        // Refresh Token
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }
        public DateTime? LastLogin { get; private set; }

        private User() { }

        public User(string email, string userName, string firstName, string lastName, string passwordHash, Guid roleId)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new DomainException("EMAIL_REQUIRED", "Correo requerido");
            if (string.IsNullOrWhiteSpace(userName)) throw new DomainException("USERNAME_REQUIRED", "Nombre de usuario requerido");

            Id = Guid.NewGuid();
            Email = email.ToLower().Trim();
            UserName = userName.ToLower().Trim();
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            PasswordHash = passwordHash;
            RoleId = roleId;
            EmailConfirmed = false;
            EmailConfirmationToken = Guid.NewGuid().ToString("N");
            IsDeleted = true;
        }

        #region Reglas de Negocio
        public void Activate()
        {
            if (IsDeleted)
                throw new DomainException("USER_ALREADY_ACTIVE", "El usuario ya está activo.");

            // Aquí podrías validar que el email esté confirmado si fuera requisito
            // if (!EmailConfirmed) throw new DomainException("EMAIL_NOT_CONFIRMED", "No se puede activar sin email confirmado.");

            IsDeleted = true;
        }

        public void Deactivate()
        {
            IsDeleted = true;
            RevokeRefreshToken(); // Por seguridad, si lo desactivan, quitamos el acceso
        }

        public void UpdateProfile(string firstName, string lastName, string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("NAME_REQUIRED", "Nombre y apellido requeridos");

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            PhoneNumber = phoneNumber;
        }

        public void RegisterLogin()
        {
            LastLogin = DateTime.UtcNow;
        }

        public void ConfirmEmail(string token)
        {
            if (EmailConfirmed)
                throw new DomainException("EMAIL_ALREADY_CONFIRMED", "Correo ya confirmado");

            if (EmailConfirmationToken != token)
                throw new DomainException("INVALID_CONFIRMATION_TOKEN", "Token inválido", "El token de confirmación no coincide.");

            EmailConfirmed = true;
            EmailConfirmationToken = null; // Limpiamos el token una vez usado
        }

        public void UpdateRefreshToken(string newToken, DateTime expiryTime)
        {
            if (string.IsNullOrWhiteSpace(newToken))
                throw new DomainException("INVALID_REFRESH_TOKEN", "Token inválido");

            RefreshToken = newToken;
            RefreshTokenExpiryTime = expiryTime;
        }

        public void RevokeRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiryTime = null;
        }

        public bool IsRefreshTokenValid(string token)
        {
            return RefreshToken == token && RefreshTokenExpiryTime > DateTime.UtcNow;
        }

        public void AssignRole(Guid newRoleId)
        {
            if (newRoleId == Guid.Empty)
                throw new DomainException("INVALID_ROLE_ID", "Rol inválido");

            RoleId = newRoleId;
        }

        #endregion
    }
}