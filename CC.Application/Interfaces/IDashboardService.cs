using CC.Application.Common.Bases;
using CC.Application.DTOs.Dashboard;

namespace CC.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<BaseResponse<ProjectDashboardDto>> GetUserDashboardAsync();
    }
}
