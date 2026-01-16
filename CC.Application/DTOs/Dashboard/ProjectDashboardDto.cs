namespace CC.Application.DTOs.Dashboard
{
    public record ProjectDashboardDto(
        int TotalProjects,
        int ActiveProjects,
        int TotalApiKeys,
        int TotalAssignedCatalogs,
        List<CatalogUsageDto> CatalogsPerProject,
        List<RecentActivityDto> RecentActivity
    );

    public record CatalogUsageDto(
        string ProjectName,
        int CatalogCount
    );

    public record RecentActivityDto(
        string EntityName,
        string Action,
        DateTime Date
    );
}
