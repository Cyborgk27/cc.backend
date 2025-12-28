using CC.Api.Middleware;
using CC.Application.DTOs.Project.CC.Application.DTOs.Project;
using CC.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CC.Api.Controllers
{
    [Route("api/projects")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectApplication _projectApp;

        public ProjectController(IProjectApplication projectApp)
        {
            _projectApp = projectApp;
        }

        [HttpGet]
        [Permission("PROJECT", "READ")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? name = null)
        {
            var response = await _projectApp.GetPagedProjectsAsync(page, size, name);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id:guid}")]
        [Permission("PROJECT", "READ")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _projectApp.GetProjectByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("save")]
        [Permission("PROJECT", "CREATE")]
        public async Task<IActionResult> Save([FromBody] ProjectDto request)
        {
            var response = await _projectApp.SaveProjectAsync(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
