using CC.Application.DTOs.Auth;
using CC.Application.DTOs.User;
using CC.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CC.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthApplication _authApplication;

        public AuthController(IAuthApplication authApplication)
        {
            _authApplication = authApplication;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authApplication.LoginAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserDto request)
        {
            var response = await _authApplication.RegisterAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var response = await _authApplication.RefreshTokenAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return BadRequest("Faltan parámetros de confirmación.");

            var response = await _authApplication.ConfirmEmailAsync(email, token);

            // Si la confirmación es exitosa, podrías incluso redirigir a una página de éxito en el Front
            return StatusCode(response.StatusCode, response);
        }
    }
}
