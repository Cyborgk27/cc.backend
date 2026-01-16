using CC.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CC.Api.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Obtiene las métricas principales del usuario logueado
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var response = await _dashboardService.GetUserDashboardAsync();

            // Gracias a ServiceData, el statusCode ya viene configurado (200, 403, etc.)
            return StatusCode(response.StatusCode, response);
        }
    }
}
