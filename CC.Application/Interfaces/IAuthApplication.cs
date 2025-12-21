using CC.Application.Common.Bases;
using CC.Application.DTOs.Auth;
using CC.Application.DTOs.User;

namespace CC.Application.Interfaces
{
    public interface IAuthApplication
    {
        Task<BaseResponse<AuthResponse>> LoginAsync(LoginRequest request);
        Task<BaseResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
        Task<BaseResponse<Guid>> RegisterAsync(UserDto request);
        Task<BaseResponse<bool>> ConfirmEmailAsync(string email, string token);
    }
}
