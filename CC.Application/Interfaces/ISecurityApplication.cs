using CC.Application.Common.Bases;
using CC.Application.DTOs.Security;

namespace CC.Application.Interfaces
{
    public interface ISecurityApplication
    {
        Task<BaseResponse<bool>> AssignPermissionsToRoleAsync(Guid roleId, List<int> permissionIds);
        Task<BaseResponse<Guid>> CreateRoleAsync(RoleDto request);
        Task<BaseResponse<int>> CreateFeatureAsync(FeatureDto request);
        Task<BaseResponse<int>> CreatePermissionAsync(PermissionDto request);
        Task<BaseResponse<IEnumerable<RoleDto>>> GetAllRolesAsync();
        Task<BaseResponse<RoleDto>> GetRoleByIdAsync(Guid id);
        Task<BaseResponse<IEnumerable<FeatureDto>>> GetAllFeaturesAsync();
        Task<BaseResponse<IEnumerable<PermissionDto>>> GetAllPermissionsAsync();
    }
}
