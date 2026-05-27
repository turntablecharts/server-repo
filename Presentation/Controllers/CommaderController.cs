using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommaderController : ControllerBase
{
    public CommaderController()
    {
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new { message = "CommaderController is working!" });
    }
}
