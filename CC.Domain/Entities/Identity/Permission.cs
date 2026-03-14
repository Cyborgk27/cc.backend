using CC.Domain.Common;
using CC.Domain.Entities.Features;
using CC.Domain.Exceptions;

namespace CC.Domain.Entities.Identity
{
    public class Permission : BaseEntity<int>
    {

        public string Name { get; private set; }
        public string ShowName { get; private set; }
        public int FeatureId { get; private set; }
        public virtual Feature Feature { get; private set; } = null!;

        // Constructor para Entity Framework
        private Permission() { }

        public Permission(string name, string showName, int featureId)
        {
            ValidateName(name);
            ValidateShowName(showName);

            if (featureId <= 0)
                throw new UserFriendlyException("El ID del feature asociado es inválido.");

            Name = name.ToUpper().Trim();
            ShowName = showName.Trim();
            FeatureId = featureId;
        }

        #region Reglas de Negocio

        public void Update(string name, string showName)
        {
            ValidateName(name);
            ValidateShowName(showName);

            Name = name.ToUpper().Trim();
            ShowName = showName.Trim();

            // Aquí podrías agregar una regla de auditoría manual si no usas el interceptor
            // this.MarkAsUpdated(userId); 
        }

        #endregion

        #region Validaciones
        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new UserFriendlyException("El nombre técnico del permiso es obligatorio.");

            // Regla: No permitir espacios en el nombre técnico (estándar de Claims)
            if (name.Contains(" "))
                throw new UserFriendlyException("El nombre técnico del permiso no puede contener espacios. Use guiones bajos (_) o camelCase.");
        }

        private void ValidateShowName(string showName)
        {
            if (string.IsNullOrWhiteSpace(showName))
                throw new UserFriendlyException("El nombre para mostrar del permiso es obligatorio.");
        }
        #endregion
    }
}