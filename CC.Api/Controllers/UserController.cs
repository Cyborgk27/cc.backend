using CC.Api.Middleware;
using CC.Application.DTOs.User;
using CC.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CC.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserApplication _userApp;

        public UserController(IUserApplication userApp)
        {
            _userApp = userApp;
        }

        [HttpGet]
        [Permission("USERS", "READ")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? search = null)
        {
            var response = await _userApp.GetPagedUsersAsync(page, size, search);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id:guid}")]
        [Permission("USERS", "READ")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _userApp.GetUserByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("save")]
        [Permission("USERS", "CREATE")]
        public async Task<IActionResult> Save([FromBody] UserDto request)
        {
            // El servicio maneja la lógica de Hash y creación/edición
            var response = await _userApp.SaveUserAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("update")]
        [Permission("USERS", "UPDATE")]
        public async Task<IActionResult> Update([FromBody] UserDto request)
        {
            var response = await _userApp.SaveUserAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("activate/{userId:guid}")]
        [Permission("USERS", "UPDATE")]
        public async Task<IActionResult> ActivateUser(Guid userId)
        {
            var response = await _userApp.ActivateUserAsync(userId);
            return StatusCode(response.StatusCode, response);
        }
    }
}