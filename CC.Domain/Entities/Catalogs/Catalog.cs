using CC.Domain.Common;
using CC.Domain.Entities.Project;
using CC.Domain.Exceptions;

namespace CC.Domain.Entities.Catalogs
{
    public class Catalog : BaseEntity<int>
    {
        public int? ParentId { get; private set; }
        public string ShowName { get; private set; }
        public string Name { get; private set; }
        public string Abbreviation { get; private set; }
        public string Value { get; private set; }
        public string? Description { get; private set; }
        public bool IsParent { get; private set; }

        public virtual Catalog? Parent { get; private set; }
        public virtual ICollection<Catalog> Children { get; private set; } = new List<Catalog>();
        public virtual ICollection<ProjectCatalog> ProjectCatalogs { get; private set; } = new List<ProjectCatalog>();

        protected Catalog() { }

        public Catalog(
            string name,
            string showName,
            string abbreviation,
            string value,
            string? description = null,
            int? parentId = null)
        {
            Validate(name, showName, value);

            Name = name.ToUpper().Trim();
            ShowName = showName.Trim();
            Abbreviation = abbreviation.Trim();
            Value = value;
            Description = description;
            ParentId = parentId;

            // Si no tiene padre, nace como un nodo raíz (Parent)
            IsParent = !parentId.HasValue;
        }

        #region Métodos de Dominio

        public void UpdateInfo(string showName, string abbreviation, string value, int? parentId, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(showName))
                throw new UserFriendlyException("El nombre para mostrar del catálogo es obligatorio.");

            ShowName = showName.Trim();
            Abbreviation = abbreviation.Trim();
            Value = value;
            Description = description;
            ParentId = parentId;
        }

        public void AssignParent(Catalog parent)
        {
            if (parent == null)
                throw new UserFriendlyException("El catálogo padre no puede ser nulo.");

            if (parent.Id == Id && Id != 0)
                throw new UserFriendlyException("Un catálogo no puede ser su propio padre.");

            Parent = parent;
            ParentId = parent.Id;
            IsParent = false; // Al tener padre, deja de ser un nodo raíz en el paginado principal
        }

        public void AddChild(Catalog child)
        {
            if (child == null)
                throw new UserFriendlyException("El catálogo hijo no puede ser nulo.");

            child.AssignParent(this);
            Children.Add(child);
            IsParent = true; // Si agrego un hijo, este nodo se confirma como padre
        }

        private void Validate(string name, string showName, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new UserFriendlyException("El nombre del catálogo es obligatorio.");

            if (string.IsNullOrWhiteSpace(showName))
                throw new UserFriendlyException("El nombre para mostrar del catálogo es obligatorio.");

            if (string.IsNullOrWhiteSpace(value))
                throw new UserFriendlyException("El valor del catálogo es obligatorio.");
        }

        #endregion
    }
}