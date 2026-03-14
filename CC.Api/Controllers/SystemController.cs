using CC.Api.Middleware;
using CC.Application.Modules.System.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CC.Api.Controllers
{
    [Route("api/system")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly IAuditApplication _auditApplication;
        public SystemController(IAuditApplication auditApplication)
        {
            _auditApplication = auditApplication;
        }

        [HttpGet]
        [Permission("LOG", "READ")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? name = null)
        {
            var res = await _auditApplication.GetPagedAuditsAsync(page, size, name);
            return StatusCode(res.StatusCode, res);
        }
    }
}
