using CC.Domain.Common;
using CC.Domain.Exceptions;

namespace CC.Domain.Entities
{
    public class Role : BaseEntity<Guid>
    {
        public string Name { get; private set; }     // "ADMIN", "SALES_MANAGER"
        public string ShowName { get; private set; } // "Administrador", "Gerente de Ventas"
        public string Description { get; private set; }
        public bool IsEnabled { get; private set; }

        // Relación con los permisos asignados
        public virtual ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

        private Role() { }

        public Role(string name, string showName, string description = "")
        {
            Validate(name, showName);

            Id = Guid.NewGuid(); // Al ser Guid, lo generamos en el constructor
            Name = name.ToUpper().Trim();
            ShowName = showName.Trim();
            Description = description;
            IsEnabled = true;
        }

        #region Reglas de Negocio

        public void UpdateDetails(string showName, string description)
        {
            if (string.IsNullOrWhiteSpace(showName))
                throw new DomainException("INVALID_ROLE_SHOWNAME", "Nombre requerido", "El nombre visible del rol no puede estar vacío.");

            ShowName = showName.Trim();
            Description = description;
        }

        public void Disable()
        {
            if (Name == "ADMIN")
                throw new DomainException("CANNOT_DISABLE_ADMIN", "Acción no permitida", "El rol de Administrador no puede ser desactivado.");

            IsEnabled = false;
        }

        private void Validate(string name, string showName)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("ROLE_NAME_REQUIRED", "Código de rol requerido", "El nombre técnico del rol es obligatorio.");

            if (string.IsNullOrWhiteSpace(showName))
                throw new DomainException("ROLE_SHOWNAME_REQUIRED", "Nombre visible requerido", "El nombre para mostrar del rol es obligatorio.");
        }

        #endregion
    }
}