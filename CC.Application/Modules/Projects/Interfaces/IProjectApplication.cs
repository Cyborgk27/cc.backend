using CC.Application.Common.Bases;
using CC.Application.DTOs.Catalog;
using CC.Application.DTOs.Project.CC.Application.DTOs.Project;

namespace CC.Application.Modules.Projects.Interfaces
{
    public interface IProjectApplication
    {
        Task<BaseResponse<IEnumerable<ProjectDto>>> GetPagedProjectsAsync(int page, int size, string? name = null);
        Task<BaseResponse<ProjectDto>> GetProjectByIdAsync(Guid id);
        Task<BaseResponse<bool>> SaveProjectAsync(ProjectDto dto);
        Task<BaseResponse<bool>> DeleteProjectAsync(Guid id);
        Task<BaseResponse<bool>> DeleteApiKey(int apiKeyId);
        Task<BaseResponse<IEnumerable<CatalogDto>>> GetPagedCatalogsAsync(int page, int size, string? name = null);
    }
}