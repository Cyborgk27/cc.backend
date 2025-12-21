using CC.Domain.Common;
using CC.Domain.Exceptions;

namespace CC.Domain.Entities
{
    public class Feature : BaseEntity<int>
    {
        public string Name { get; private set; }
        public string ShowName { get; private set; }
        public string Path { get; private set; }
        public string Icon { get; private set; }

        public virtual ICollection<Permission> AvailablePermissions { get; private set; } = new List<Permission>();

        // Constructor privado para EF
        private Feature() { }

        public Feature(string name, string showName, string path, string icon)
        {
            ValidateName(name);
            ValidatePath(path);

            Name = name.ToUpper().Trim();
            ShowName = showName.Trim();
            Path = path.ToLower().Trim();
            Icon = icon;
        }

        #region Reglas de Negocio (Comportamiento)

        public void UpdateDetails(string showName, string path, string icon)
        {
            if (string.IsNullOrWhiteSpace(showName))
                throw new DomainException("INVALID_SHOW_NAME", "Nombre visible requerido", "El nombre a mostrar no puede estar vacío.");

            ValidatePath(path);

            ShowName = showName.Trim();
            Path = path.ToLower().Trim();
            Icon = icon;
        }

        public void AddPermission(string permName, string permShowName)
        {
            // Regla: No duplicar nombres de permisos dentro del mismo feature
            if (AvailablePermissions.Any(p => p.Name == permName.ToUpper()))
            {
                throw new DomainException(
                    "DUPLICATE_PERMISSION",
                    "Permiso duplicado",
                    $"El permiso '{permName}' ya existe para el feature {this.Name}.");
            }

            AvailablePermissions.Add(new Permission(permName, permShowName, this.Id));
        }

        #endregion

        #region Validaciones Privadas
        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("EMPTY_NAME", "Nombre requerido", "El Name técnico de la funcionalidad es obligatorio.");

            if (name.Length < 3)
                throw new DomainException("NAME_TOO_SHORT", "Nombre muy corto", "El nombre técnico debe tener al menos 3 caracteres.");
        }

        private void ValidatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !path.StartsWith("/"))
                throw new DomainException(
                    "INVALID_PATH",
                    "Ruta inválida",
                    "El path debe comenzar con '/' y no puede estar vacío.");
        }
        #endregion
    }
}