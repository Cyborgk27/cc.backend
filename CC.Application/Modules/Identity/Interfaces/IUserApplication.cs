using CC.Application.Common.Bases;
using CC.Application.Modules.Identity.Dtos;

namespace CC.Application.Modules.Identity.Interfaces
{
    public interface IUserApplication
    {
        Task<BaseResponse<IEnumerable<UserDto>>> GetPagedUsersAsync(int page, int size, string? search = null);
        Task<BaseResponse<UserDto>> GetUserByIdAsync(Guid id);
        Task<BaseResponse<bool>> SaveUserAsync(RegisterRequest dto);
        Task<BaseResponse<bool>> ActivateUserAsync(Guid userId);
        Task<BaseResponse<bool>> DeactivateUserAsync(Guid userId);
    }
}
