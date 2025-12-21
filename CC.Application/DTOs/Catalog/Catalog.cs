namespace CC.Application.DTOs.Catalog
{
    public record CatalogDto(
        int? Id,
        int? ParentId,
        string Name,
        string ShowName,
        string Abbreviation,
        string Value,
        string? Description,
        bool IsParent,
        bool IsActive,
        bool IsDeleted = false,
        IEnumerable<CatalogDto>? Children = null // Para recursividad
    );
}
