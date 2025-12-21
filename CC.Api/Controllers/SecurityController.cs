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
    }
}