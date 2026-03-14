using CC.Domain.Common;
using CC.Domain.Entities.Catalogs;

namespace CC.Domain.Entities.Project
{
    public class ProjectCatalog : BaseEntity<int>
    {
        public Guid ProjectId { get; private set; }
        public int CatalogId { get; private set; }

        // Propiedades de navegación
        public virtual Project Project { get; private set; } = null!;
        public virtual Catalog Catalog { get; private set; } = null!;

        protected ProjectCatalog() { }

        public ProjectCatalog(Guid projectId, int catalogId)
        {
            ProjectId = projectId;
            CatalogId = catalogId;
        }
    }
}