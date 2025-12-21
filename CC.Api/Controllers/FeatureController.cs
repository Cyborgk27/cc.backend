using CC.Application.DTOs.Features;
using CC.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CC.Api.Controllers
{
    [Route("api/features")]
    [Authorize]
    [ApiController]
    public class FeatureController : ControllerBase
    {
        private readonly IFeatureApplication _featureApplication;

        public FeatureController(IFeatureApplication featureApplication)
        {
            _featureApplication = featureApplication;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? name = null)
        {
            var response = await _featureApplication.GetPagedFeaturesAsync(page, size, name);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _featureApplication.GetFeatureByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFeatureRequest request)
        {
            var response = await _featureApplication.CreateFeatureAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateFeatureRequest request)
        {
            var response = await _featureApplication.UpdateFeatureAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _featureApplication.DeleteFeatureAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
