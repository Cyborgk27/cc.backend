using CC.Domain.Common;
using CC.Domain.Exceptions;

namespace CC.Domain.Entities
{
    public class Project : BaseEntity<Guid>
    {
        public string Name { get; private set; }
        public string ShowName { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }

        // Relación con los catálogos asignados
        private readonly List<ProjectCatalog> _projectCatalogs = new();
        public virtual ICollection<ProjectCatalog> ProjectCatalogs => _projectCatalogs.AsReadOnly();

        private readonly List<ProjectApiKey> _apiKeys = new();
        public virtual ICollection<ProjectApiKey> ApiKeys => _apiKeys.AsReadOnly();

        protected Project() { }

        public Project(string name, string showName, string description = "")
        {
            Validate(name, showName);

            Id = Guid.NewGuid();
            Name = name.ToUpper().Trim();
            ShowName = showName.Trim();
            Description = description;
            IsActive = true;
        }

        #region Métodos de Dominio

        public void UpdateInfo(string name, string showName, string description)
        {
            if (string.IsNullOrWhiteSpace(showName))
                throw new DomainException("PROJECT_SHOWNAME_REQUIRED", "Nombre requerido", "El nombre visible del proyecto no puede estar vacío.");
            Name = name.ToUpper().Trim();
            ShowName = showName.Trim();
            Description = description;
        }

        public void AssignCatalog(int catalogId)
        {
            // Validamos que no se asigne el mismo catálogo dos veces
            if (_projectCatalogs.Any(pc => pc.CatalogId == catalogId))
                throw new DomainException("CATALOG_ALREADY_ASSIGNED", "Catálogo duplicado", "Este catálogo ya está asignado al proyecto.");

            _projectCatalogs.Add(new ProjectCatalog(this.Id, catalogId));
        }

        public void RemoveCatalog(int catalogId)
        {
            var assignment = _projectCatalogs.FirstOrDefault(pc => pc.CatalogId == catalogId);
            if (assignment == null)
                throw new DomainException("CATALOG_NOT_ASSIGNED", "No encontrado", "El catálogo que intenta remover no está asignado a este proyecto.");

            _projectCatalogs.Remove(assignment);
        }

        private void Validate(string name, string showName)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("PROJECT_NAME_REQUIRED", "Código requerido", "El nombre técnico del proyecto es obligatorio.");

            if (string.IsNullOrWhiteSpace(showName))
                throw new DomainException("PROJECT_SHOWNAME_REQUIRED", "Nombre requerido", "El nombre para mostrar del proyecto es obligatorio.");
        }

        public void GenerateApiKey(string title, string description, DateTime? expiration, bool isIndefinite, string? ip = null, string? domain = null)
        {
            var apiKey = new ProjectApiKey(this.Id, title, description, expiration, isIndefinite, ip, domain);
            _apiKeys.Add(apiKey);
        }

        #endregion
    }
}