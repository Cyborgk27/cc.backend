using CC.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CC.Api.Controllers
{
    [Route("api/common")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly ICommonApplication _commonApplication;

        public CommonController(ICommonApplication commonApplication)
        {
            _commonApplication = commonApplication;
        }

        [HttpGet("roles")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _commonApplication.GetRoles();
            return Ok(roles);
        }

        [HttpGet("catalogs-parents")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCatalogsParent()
        {
            var catalogs = await _commonApplication.GetCatalogsParents();
            return Ok(catalogs);
        }
    }
}