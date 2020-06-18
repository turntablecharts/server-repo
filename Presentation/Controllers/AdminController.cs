using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Infrastructure;
using Infrastructure.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Presentation.Controllers {
    [Authorize]
    [ApiController]
    [Route ("api/[controller]")]
    public class AdminController : ControllerBase {

        private IGenericRepository<Log> _logRepo;
        private IGenericRepository<TtcUser> _userGenericRepo;

        public AdminController (
            IGenericRepository<Log> logRepo, IGenericRepository<TtcUser> userGenericRepo) {
            _logRepo = logRepo;

            _userGenericRepo = userGenericRepo;
        }

        [HttpGet ("users/all")]
        public IActionResult GetAllUsers () {
            //var result = await _userRepo.GetAllUsers();
            var result = _userGenericRepo.GetAll ();
            return Ok (result);
        }

        [HttpGet ("users/{id}")]
        public IActionResult GetOneUser ([FromRoute] int id) {
            var user = _userGenericRepo.GetById (id);
            if (user == null) {
                return NotFound ();
            }

            return Ok (user);
        }

        [HttpDelete ("users/delete/{id}")]
        public IActionResult DeleteUser ([FromRoute] int id) {
            var user = _userGenericRepo.GetById (id);
            _userGenericRepo.Delete (user);
            return Ok ("sucessfully deleted");
        }

        [HttpGet ("logs")]
        public IActionResult GetLog () {
            var log = _logRepo.GetAll ();

            return Ok (log);
        }
    }
}