namespace CC.Application.DTOs.External
{
    public record ExternalCatalogResponse(
            string Name,
            string ShowName,
            string Value,
            string Abbreviation,
            string? Description
        );
}
