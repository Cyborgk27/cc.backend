using CC.Domain.Common;
using CC.Domain.Exceptions;

namespace CC.Domain.Entities
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
                throw new DomainException("INVALID_FEATURE_ID", "Feature Inválido", "El permiso debe estar asociado a un Feature existente.");

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
                throw new DomainException("PERMISSION_NAME_REQUIRED", "Código de permiso requerido", "El nombre técnico del permiso no puede estar vacío.");

            // Regla: No permitir espacios en el nombre técnico (estándar de Claims)
            if (name.Contains(" "))
                throw new DomainException("INVALID_PERMISSION_FORMAT", "Formato de permiso inválido", "El nombre técnico no debe contener espacios (use guiones bajos).");
        }

        private void ValidateShowName(string showName)
        {
            if (string.IsNullOrWhiteSpace(showName))
                throw new DomainException("PERMISSION_SHOWNAME_REQUIRED", "Nombre de visualización requerido", "Debe indicar cómo se mostrará el permiso al usuario.");
        }
        #endregion
    }
}