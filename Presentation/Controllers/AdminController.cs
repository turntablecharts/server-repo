using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private ITtcUserRepo _userRepo;
        private ILogRepo _logRepo;
        public AdminController(ITtcUserRepo userRepo,
            ILogRepo logRepo)
        {
            _logRepo = logRepo;
            _userRepo = userRepo;
        }


        [HttpGet("users/all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userRepo.GetAllUsers();

            return Ok(result);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetOneUser([FromRoute]int id)
        {
            var user = await _userRepo.GetUser(id);
            if(user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpDelete("users/delete/{id}")]
        public IActionResult EditUser([FromBody] TtcUser user, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var editedUser = _userRepo.Edit(user, id);

            return Ok(editedUser);
        }


        [HttpGet("logs")]
        public async Task<IActionResult> GetLog()
        {
            var log = await _logRepo.GetLogs();

            return Ok(log);
        }
    }
}
