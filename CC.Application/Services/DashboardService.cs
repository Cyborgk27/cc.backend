using CC.Application.Common.Bases;
using CC.Application.Common.Helpers;
using CC.Application.DTOs.Dashboard;
using CC.Application.Interfaces;
using CC.Domain.Repositories;
using CC.Utilities.Static;

namespace CC.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly ServiceData _serviceData; // Inyectamos tu Helper

    public DashboardService(IUnitOfWork unitOfWork, IUserContext userContext, ServiceData serviceData)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _serviceData = serviceData;
    }

    public async Task<BaseResponse<ProjectDashboardDto>> GetUserDashboardAsync()
    {
        var currentUserId = _userContext.UserId;

        // 1. Obtener proyectos con sus relaciones (API Keys y Catálogos asignados)
        var projects = await _unitOfWork.Projects.GetAsync(
            filter: p => p.AuditCreateUser == currentUserId && !p.AuditDeleteDate.HasValue,
            includeProperties: "ProjectCatalogs,ApiKeys"
        );

        // 2. Preparar los listados para el Record
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

        // 3. Crear el record de datos
        var dashboardData = new ProjectDashboardDto(
            TotalProjects: projects.Count(),
            ActiveProjects: projects.Count(p => p.IsActive),
            TotalApiKeys: projects.Sum(p => p.ApiKeys.Count),
            TotalAssignedCatalogs: projects.Sum(p => p.ProjectCatalogs.Count),
            CatalogsPerProject: catalogsUsage,
            RecentActivity: recentActivity
        );

        // 4. USAR SERVICEDATA: Esto llena automáticamente IsSuccess, StatusCodeCat y el Count
        return _serviceData.CreateResponse(
            data: dashboardData,
            message: ReplyMessage.MESSAGE_QUERY,
            statusCode: 200
        );
    }
}