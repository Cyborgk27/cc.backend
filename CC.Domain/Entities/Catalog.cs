using CC.Domain.Common;
using CC.Domain.Exceptions;

namespace CC.Domain.Entities
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

        public void UpdateInfo(string showName, string abbreviation, string value, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(showName))
                throw new DomainException("CATALOG_SHOWNAME_REQUIRED", "Nombre requerido", "El nombre visible no puede estar vacío.");

            ShowName = showName.Trim();
            Abbreviation = abbreviation.Trim();
            Value = value;
            Description = description;
        }

        public void AssignParent(Catalog parent)
        {
            if (parent == null)
                throw new DomainException("PARENT_REQUIRED", "Padre requerido", "Debe proporcionar un catálogo válido para asignar como padre.");

            if (parent.Id == this.Id && this.Id != 0)
                throw new DomainException("CIRCULAR_REFERENCE", "Referencia circular", "Un catálogo no puede ser padre de sí mismo.");

            Parent = parent;
            ParentId = parent.Id;
            IsParent = false; // Al tener padre, deja de ser un nodo raíz en el paginado principal
        }

        public void AddChild(Catalog child)
        {
            if (child == null)
                throw new DomainException("CHILD_REQUIRED", "Hijo requerido", "El catálogo hijo no puede ser nulo.");

            child.AssignParent(this);
            Children.Add(child);
            this.IsParent = true; // Si agrego un hijo, este nodo se confirma como padre
        }

        private void Validate(string name, string showName, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("CATALOG_NAME_REQUIRED", "Código requerido", "El nombre técnico (Name) es obligatorio.");

            if (string.IsNullOrWhiteSpace(showName))
                throw new DomainException("CATALOG_SHOWNAME_REQUIRED", "Nombre visible requerido", "El nombre para mostrar es obligatorio.");

            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("CATALOG_VALUE_REQUIRED", "Valor requerido", "El valor del catálogo no puede estar vacío.");
        }

        #endregion
    }
}