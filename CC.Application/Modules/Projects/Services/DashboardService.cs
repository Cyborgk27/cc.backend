using CC.Application.Common.Bases;
using CC.Application.Common.Helpers;
using CC.Application.DTOs.Dashboard;
using CC.Application.Modules.Identity.Interfaces;
using CC.Application.Modules.Projects.Interfaces;
using CC.Domain.Repositories;
using CC.Utilities.Static;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ServiceData _serviceData;

    public DashboardService(IUnitOfWork unitOfWork, IUserContext userContext, ServiceData serviceData)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _serviceData = serviceData;
    }

    public async Task<BaseResponse<ProjectDashboardDto>> GetUserDashboardAsync()
    {
        var isAdmin = _userContext.IsInRole("ADMINISTRATOR");
        var currentUserId = _userContext.UserId;

        // 1. Obtener Proyectos (filtrados por seguridad)
        var projects = await _unitOfWork.Projects.GetAsync(
            filter: p => !p.IsDeleted && (isAdmin || p.AuditCreateUser == currentUserId),
            includeProperties: "ApiKeys,ProjectCatalogs"
        );

        // 2. Obtener Catálogos Existentes (Maestra real)
        // Consultamos la tabla de catálogos directamente
        var allCatalogs = await _unitOfWork.Catalogs.GetAsync(c => !c.IsDeleted && c.IsParent);

        // 3. Preparar estadísticas de catálogos reales
        // Por ejemplo: ¿Cuántos proyectos están usando cada catálogo existente?
        var catalogsUsage = allCatalogs.Select(c => new CatalogUsageDto(
            c.Name,
            // Contamos en cuántos de los proyectos filtrados aparece este catálogo
            projects.Count(p => p.ProjectCatalogs.Any(pc => pc.CatalogId == c.Id))
        )).ToList();

        // 4. Actividad reciente de proyectos
        var recentActivity = projects
            .OrderByDescending(p => p.AuditUpdateDate ?? p.AuditCreateDate)
            .Take(5)
            .Select(p => new RecentActivityDto(
                p.ShowName,
                p.AuditUpdateDate.HasValue ? "Actualizado" : "Creado",
                p.AuditUpdateDate ?? p.AuditCreateDate
            )).ToList();

        // 5. Construcción del DTO final
        var dashboardData = new ProjectDashboardDto(
            TotalProjects: projects.Count(),
            ActiveProjects: projects.Count(p => !p.IsDeleted),
            TotalApiKeys: projects.Sum(p => p.ApiKeys.Count(a => !a.IsDeleted)),

            // CAMBIO: Ahora refleja el total de la maestra de catálogos
            TotalAssignedCatalogs: allCatalogs.Count(),

            CatalogsPerProject: catalogsUsage, // Distribución de uso por catálogo
            RecentActivity: recentActivity
        );

        return _serviceData.CreateResponse(
            data: dashboardData,
            message: ReplyMessage.MESSAGE_QUERY,
            statusCode: 200
        );
    }
}