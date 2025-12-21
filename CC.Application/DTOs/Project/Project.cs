namespace CC.Application.DTOs.Project
{
    namespace CC.Application.DTOs.Project
    {
        public record ProjectApiKeyDto(
            int? Id,                 // Null/0 = Nuevo
            string Title,
            string Description,
            string? Key,             // Solo lectura (Response)
            DateTime? ExpirationDate,
            bool IsIndefinite,
            string? AllowedIp,
            string? AllowedDomain,
            bool IsDeleted = false   // Si viene en true, la borramos
        );

        public record ProjectDto(
            Guid? Id,                // Null = Nuevo Proyecto
            string Name,
            string ShowName,
            string Description,
            bool IsActive,
            IEnumerable<ProjectApiKeyDto> ApiKeys,
            IEnumerable<int>? CatalogIds = null
        );
    }
}