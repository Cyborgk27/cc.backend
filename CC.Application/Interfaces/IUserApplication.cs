using CC.Application.Common.Bases;
using CC.Application.DTOs.User;

namespace CC.Application.Interfaces
{
    public interface IUserApplication
    {
        Task<BaseResponse<IEnumerable<UserDto>>> GetPagedUsersAsync(int page, int size, string? search = null);
        Task<BaseResponse<UserDto>> GetUserByIdAsync(Guid id);
        Task<BaseResponse<bool>> SaveUserAsync(UserDto dto);
    }
}
