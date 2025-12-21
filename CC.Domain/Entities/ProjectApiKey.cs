using CC.Domain.Common;
using CC.Domain.Exceptions;

namespace CC.Domain.Entities
{
    public class ProjectApiKey : BaseEntity<int>
    {
        public Guid ProjectId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Key { get; private set; } // El hash o string de la API Key
        public DateTime? ExpirationDate { get; private set; }
        public bool IsIndefinite { get; private set; }
        public bool IsEnabled { get; private set; }

        // Restricciones de seguridad
        public string? AllowedIp { get; private set; }     // Ejemplo: "192.168.1.1"
        public string? AllowedDomain { get; private set; } // Ejemplo: "mi-cliente.com"

        public virtual Project Project { get; private set; } = null!;

        protected ProjectApiKey() { }

        public ProjectApiKey(
            Guid projectId,
            string title,
            string description,
            DateTime? expirationDate,
            bool isIndefinite,
            string? allowedIp = null,
            string? allowedDomain = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("APIKEY_TITLE_REQUIRED", "Título requerido", "El título de la API Key es obligatorio.");

            if (!isIndefinite && (expirationDate == null || expirationDate <= DateTime.Now))
                throw new DomainException("INVALID_EXPIRATION", "Fecha inválida", "Si la llave no es indefinida, debe tener una fecha de expiración futura.");

            ProjectId = projectId;
            Title = title.Trim();
            Description = description;
            ExpirationDate = isIndefinite ? null : expirationDate;
            IsIndefinite = isIndefinite;
            AllowedIp = allowedIp;
            AllowedDomain = allowedDomain;
            IsEnabled = true;

            // Generamos una llave única (puedes usar un generador más complejo luego)
            Key = $"CC-{Guid.NewGuid().ToString("N").ToUpper()}";
        }

        #region Métodos de Dominio

        public void Revoke()
        {
            IsEnabled = false;
        }

        public bool IsValid()
        {
            if (!IsEnabled) return false;
            if (IsIndefinite) return true;
            return ExpirationDate > DateTime.Now;
        }

        public void UpdateSecurity(string? ip, string? domain)
        {
            AllowedIp = ip;
            AllowedDomain = domain;
        }

        #endregion
    }
}