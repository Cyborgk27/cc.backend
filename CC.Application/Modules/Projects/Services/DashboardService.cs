using CC.Application.Common.Bases;
using CC.Application.Common.Helpers;
using CC.Application.DTOs.Dashboard;
using CC.Application.Modules.Identity.Interfaces;
using CC.Application.Modules.Projects.Interfaces;
using CC.Domain.Repositories;
using CC.Utilities.Static;

namespace CC.Application.Modules.Projects.Services;

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
        // 1. Lógica de filtrado: Si es Admin ve todo, si no, solo lo suyo
        var isAdmin = _userContext.IsInRole("ADMINISTRATOR");
        var currentUserId = _userContext.UserId;

        var projects = await _unitOfWork.Projects.GetAsync(
            filter: p => !p.IsDeleted && (isAdmin || p.AuditCreateUser == currentUserId),
            includeProperties: "ProjectCatalogs,ApiKeys"
        );

        // 2. Preparar los listados para el DTO
        var catalogsUsage = projects.Select(p => new CatalogUsageDto(
            p.ShowName,
            p.ProjectCatalogs.Count
        )).ToList();

        var recentActivity = projects
            .OrderByDescending(p => p.AuditUpdateDate ?? p.AuditCreateDate)
            .Take(5)
            .Select(p => new RecentActivityDto(
                p.ShowName,
                p.AuditUpdateDate.HasValue ? "Actualizado" : "Creado",
                p.AuditUpdateDate ?? p.AuditCreateDate
            )).ToList();

        // 3. Cálculo de métricas basadas en la lista filtrada
        var dashboardData = new ProjectDashboardDto(
            TotalProjects: projects.Count(),
            ActiveProjects: projects.Count(p => !p.IsDeleted), // Ya filtrados, pero mantenemos lógica
            TotalApiKeys: projects.Sum(p => p.ApiKeys.Count(a => !a.IsDeleted)),
            TotalAssignedCatalogs: projects.Sum(p => p.ProjectCatalogs.Count),
            CatalogsPerProject: catalogsUsage,
            RecentActivity: recentActivity
        );

        // 4. Retorno usando tu Helper ServiceData
        return _serviceData.CreateResponse(
            data: dashboardData,
            message: ReplyMessage.MESSAGE_QUERY,
            statusCode: 200
        );
    }
}