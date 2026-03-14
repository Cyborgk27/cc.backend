using CC.Application.Common.Bases;
using CC.Application.Modules.Identity.Dtos;

namespace CC.Application.Modules.Identity.Interfaces
{
    public interface IAuthApplication
    {
        Task<BaseResponse<AuthResponse>> LoginAsync(LoginRequest request);
        Task<BaseResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
        Task<BaseResponse<Guid>> RegisterAsync(UserDto request);
        Task<BaseResponse<bool>> ConfirmEmailAsync(string email, string token);
    }
}
