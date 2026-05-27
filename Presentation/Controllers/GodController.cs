using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class GodController : ControllerBase
    {
        private readonly ICacheService _cacheService;

        public GodController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [HttpGet("cache/clear")]
        public IActionResult ClearCache([FromQuery] string secretKey)
        {
            try
            {
                if (secretKey != "Y1pY0Dhi16VfiE6sJG0Z2354Otsz7h4I")
                {
                    return Unauthorized(new { error = "Invalid secret key" });
                }
                _cacheService.ClearAll();
                return Ok(new { message = "All cache entries have been successfully cleared" });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new { error = "An error occurred while clearing cache", details = ex.Message }
                );
            }
        }
    }
}
