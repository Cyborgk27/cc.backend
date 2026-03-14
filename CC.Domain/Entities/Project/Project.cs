using CC.Domain.Common;
using CC.Domain.Exceptions;

namespace CC.Domain.Entities.Project
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
                throw new UserFriendlyException("El nombre para mostrar del proyecto es obligatorio.");

            Name = name.ToUpper().Trim();
            ShowName = showName.Trim();
            Description = description;
        }

        public void AssignCatalog(int catalogId)
        {
            // Validamos que no se asigne el mismo catálogo dos veces
            if (_projectCatalogs.Any(pc => pc.CatalogId == catalogId))
                throw new UserFriendlyException("El catálogo ya está asignado a este proyecto.");

            _projectCatalogs.Add(new ProjectCatalog(Id, catalogId));
        }

        public void RemoveCatalog(int catalogId)
        {
            var assignment = _projectCatalogs.FirstOrDefault(pc => pc.CatalogId == catalogId);
            if (assignment == null)
                throw new UserFriendlyException("El catálogo no está asignado a este proyecto.");

            _projectCatalogs.Remove(assignment);
        }

        private void Validate(string name, string showName)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new UserFriendlyException("El nombre del proyecto es obligatorio.");

            if (string.IsNullOrWhiteSpace(showName))
                throw new UserFriendlyException("El nombre para mostrar del proyecto es obligatorio.");
        }

        public void GenerateApiKey(string title, string description, DateTime? expiration, bool isIndefinite, string? ip = null, string? domain = null)
        {
            var apiKey = new ProjectApiKey(Id, title, description, expiration, isIndefinite, ip, domain);
            _apiKeys.Add(apiKey);
        }

        #endregion
    }
}