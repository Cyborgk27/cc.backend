using CC.Domain.Common;
using CC.Domain.Exceptions;

namespace CC.Domain.Entities
{
    public class RolePermission : BaseEntity<int>
    {
        public Guid RoleId { get; private set; }
        public virtual Role Role { get; private set; } = null!;
        public int PermissionId { get; private set; }

        // Propiedad de navegación
        public virtual Permission Permission { get; private set; } = null!;

        // Constructor para Entity Framework
        private RolePermission() { }

        public RolePermission(Guid roleId, int permissionId)
        {
            ValidateAssignment(roleId, permissionId);

            RoleId = roleId;
            PermissionId = permissionId;
        }

        #region Reglas de Negocio

        private void ValidateAssignment(Guid roleId, int permissionId)
        {
            // Regla: El ID del rol no puede ser vacío
            if (roleId == Guid.Empty)
            {
                throw new DomainException(
                    "INVALID_ROLE_ID",
                    "Rol no válido",
                    "No se puede asignar un permiso a un identificador de rol vacío.");
            }

            // Regla: El ID del permiso debe ser válido
            if (permissionId <= 0)
            {
                throw new DomainException(
                    "INVALID_PERMISSION_ID",
                    "Permiso no válido",
                    "El identificador del permiso debe ser un número positivo válido.");
            }
        }

        /// <summary>
        /// Permite cambiar el permiso asignado si fuera necesario, 
        /// manteniendo las validaciones de integridad.
        /// </summary>
        public void ChangePermission(int newPermissionId)
        {
            if (newPermissionId <= 0)
                throw new DomainException("INVALID_PERMISSION_ID", "Permiso no válido", "El nuevo permiso debe ser válido.");

            PermissionId = newPermissionId;
        }

        #endregion
    }
}