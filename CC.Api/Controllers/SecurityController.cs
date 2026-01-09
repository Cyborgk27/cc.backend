using CC.Api.Middleware;
using CC.Application.DTOs.Security;
using CC.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CC.Api.Controllers
{
    [Route("api/security")]
    [ApiController]
    [Authorize]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityApplication _securityApp;

        public SecurityController(ISecurityApplication securityApp)
        {
            _securityApp = securityApp;
        }

        [HttpPost("feature/save")]
        [Permission("SECURITY", "CREATE")] // Solo quienes pueden crear seguridad
        public async Task<IActionResult> SaveFeature([FromBody] FeatureDto request)
        {
            var response = await _securityApp.CreateFeatureAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("permission/save")]
        [Permission("SECURITY", "CREATE")] // Asignado: para crear nuevos permisos técnicos
        public async Task<IActionResult> SavePermission([FromBody] PermissionDto request)
        {
            var response = await _securityApp.CreatePermissionAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("role/save")]
        [Permission("SECURITY", "CREATE")]
        public async Task<IActionResult> SaveRole([FromBody] RoleDto request)
        {
            var response = await _securityApp.CreateRoleAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("role/{id:guid}/assign-permissions")]
        [Permission("SECURITY", "UPDATE")] // Solo para actualizar relaciones existentes
        public async Task<IActionResult> AssignPermissions(Guid id, [FromBody] List<int> permissionIds)
        {
            var response = await _securityApp.AssignPermissionsToRoleAsync(id, permissionIds);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("role/all")]
        [Permission("SECURITY", "READ")]
        public async Task<IActionResult> GetAllRoles()
        {
            var response = await _securityApp.GetAllRolesAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("role/{id:guid}")]
        [Permission("SECURITY", "READ")]
        public async Task<IActionResult> GetRoleById(Guid id)
        {
            var response = await _securityApp.GetRoleByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("feature/all")]
        // [Permission("SECURITY", "READ")]
        public async Task<IActionResult> GetAllFeatures()
        {
            var response = await _securityApp.GetAllFeaturesAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("permission/all")]
        [Permission("SECURITY", "READ")]
        public async Task<IActionResult> GetAllPermissions()
        {
            var response = await _securityApp.GetAllPermissionsAsync();
            return StatusCode(response.StatusCode, response);
        }
    }
}