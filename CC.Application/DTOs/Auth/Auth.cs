namespace CC.Application.DTOs.Auth
{
    public record LoginRequest(string Email, string Password);
    public record RefreshTokenRequest(string Token, string RefreshToken);
    public record AuthResponse(
        string Token,
        string RefreshToken,
        string Email,
        string UserName,
        string FullName,
        IEnumerable<string> Roles,
        IEnumerable<string> Permissions,
        IEnumerable<NavigationDto> Navigation
    );
    public record RegisterRequest(
        string Email,
        string Password,
        string ConfirmPassword,
        Guid RoleId
    );
}
