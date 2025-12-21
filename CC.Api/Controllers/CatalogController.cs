using CC.Api.Middleware;
using CC.Application.DTOs.Catalog;
using CC.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CC.Api.Controllers
{
    [Route("api/catalogs")]
    [ApiController]
    [Authorize]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogApplication _catalogApp;

        public CatalogController(ICatalogApplication catalogApp)
        {
            _catalogApp = catalogApp;
        }

        [HttpGet]
        [Permission("CATALOGS", "READ")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? name = null)
        {
            var response = await _catalogApp.GetPagedCatalogsAsync(page, size, name);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id:int}")]
        [Permission("CATALOGS", "READ")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _catalogApp.GetCatalogByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("save")]
        [Permission("CATALOGS", "CREATE")]
        public async Task<IActionResult> Save([FromBody] CatalogDto request)
        {
            // Nota: Usamos CREATE para guardar, el service ya maneja si es nuevo o update interno
            var response = await _catalogApp.SaveCatalogAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("update")]
        [Permission("CATALOGS", "UPDATE")]
        public async Task<IActionResult> Update([FromBody] CatalogDto request)
        {
            var response = await _catalogApp.SaveCatalogAsync(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}