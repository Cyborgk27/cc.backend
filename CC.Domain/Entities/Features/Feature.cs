using CC.Domain.Common;
using CC.Domain.Entities.Identity;
using CC.Domain.Exceptions;

namespace CC.Domain.Entities.Features
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
                throw new UserFriendlyException("El nombre para mostrar del feature es obligatorio.");

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
                throw new UserFriendlyException($"El permiso '{permName}' ya existe para este feature.");
            }

            AvailablePermissions.Add(new Permission(permName, permShowName, Id));
        }

        #endregion

        #region Validaciones Privadas
        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new UserFriendlyException("El nombre técnico del feature es obligatorio.");

            if (name.Length < 3)
                throw new UserFriendlyException("El nombre técnico del feature debe tener al menos 3 caracteres.");
        }

        private void ValidatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !path.StartsWith("/"))
                throw new UserFriendlyException("La ruta del feature debe comenzar con '/'.");
        }
        #endregion
    }
}