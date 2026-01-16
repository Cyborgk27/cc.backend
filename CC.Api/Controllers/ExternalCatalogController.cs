using CC.Application.Interfaces;
using CC.Application.Common.Helpers;
using CC.Application.DTOs.External;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CC.Utilities.Static;

namespace CC.Api.Controllers
{
    [ApiController]
    [Route("api/external/catalogs")]
    [AllowAnonymous]
    public class ExternalCatalogsController : ControllerBase
    {
        private readonly IExternalCatalogApplication _externalApp;
        private readonly ServiceData _serviceData;

        public ExternalCatalogsController(IExternalCatalogApplication externalApp, ServiceData serviceData)
        {
            _externalApp = externalApp;
            _serviceData = serviceData;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            // 1. Intentamos recuperar el ProjectId del contexto (inyectado por el ApiKeyMiddleware)
            if (HttpContext.Items.TryGetValue("ProjectId", out var projectIdObj) && projectIdObj is Guid projectId)
            {
                var response = await _externalApp.GetCatalogsByProjectIdAsync(projectId);

                // Retornamos el BaseResponse que ya viene construido desde el Application Service
                return StatusCode(response.StatusCode, response);
            }

            // 2. Si no hay ProjectId, usamos CreateResponse con un 401
            // Tu método CreateResponse marcará IsSuccess = false automáticamente al no estar en la lista de éxitos
            var errorResponse = _serviceData.CreateResponse<object>(
                data: null!,
                message: "Acceso denegado. Se requiere una X-API-KEY válida y autorizada.",
                statusCode: StatusCodes.Status401Unauthorized
            );

            return StatusCode(errorResponse.StatusCode, errorResponse);
        }

        [HttpGet("{abbreviation}")]
        public async Task<IActionResult> GetByCode(string abbreviation)
        {
            // 1. Intentamos recuperar el ProjectId del contexto (inyectado por el ApiKeyMiddleware)
            if (HttpContext.Items.TryGetValue("ProjectId", out var projectIdObj) && projectIdObj is Guid projectId)
            {
                var response = await _externalApp.GetCatalogByCode(projectId, abbreviation);
                // Retornamos el BaseResponse que ya viene construido desde el Application Service
                return StatusCode(response.StatusCode, response);
            }
            // 2. Si no hay ProjectId, usamos CreateResponse con un 401
            var errorResponse = _serviceData.CreateResponse<object>(
                data: null!,
                message: "Acceso denegado. Se requiere una X-API-KEY válida y autorizada.",
                statusCode: StatusCodes.Status401Unauthorized
            );
            return StatusCode(errorResponse.StatusCode, errorResponse);
        }
    }
}