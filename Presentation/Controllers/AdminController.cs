using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Infrastructure;
using Infrastructure.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.Areas.Identity.Data;

namespace Presentation.Controllers {
    [Authorize]
    [ApiController]
    [Route ("api/[controller]")]
    public class AdminController : ControllerBase {

        private IGenericRepository<Log> _logRepo;
        private IGenericRepository<TtcUser> _userGenericRepo;
        private IGenericRepository<SubscribersEmail> _subscribers;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PresentationIdentityDbContext _context;
        public AdminController (
            IGenericRepository<Log> logRepo, IGenericRepository<TtcUser> userGenericRepo,
            UserManager<IdentityUser> userManager,
            IGenericRepository<SubscribersEmail> subscribers,
            PresentationIdentityDbContext context) {
            _logRepo = logRepo;
            _context = context;
            _userManager = userManager;
            _userGenericRepo = userGenericRepo;
            _subscribers = subscribers;
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

        // [HttpDelete ("users/delete/{id}")]
        // public async Task<IActionResult> DeleteUser ([FromRoute] int id) {
        //     var user = _userGenericRepo.GetById (id);
        //     _userGenericRepo.Delete (user);

        //     var login = await _userManager.FindByEmailAsync (user.Email);
        //     var rolesForUser = await _userManager.GetRolesAsync (login);

        //     var result = await _userManager.DeleteAsync(login);
        //     if (result.Succeeded)
        //     {
        //         await _userManager.RemoveFromRolesAsync(login, rolesForUser);
        //         return Ok("sucessfully deleted");
        //     }
        //     return Ok("deleted on ttc data but still present in database");
        // }

        [HttpGet ("logs")]
        public IActionResult GetLog () {
            var log = _logRepo.GetAll ();

            return Ok (log);
        }

        [HttpGet("subscribers/email")]
        public IActionResult GetSubscribersEmail()
        {
            var subscribers = _subscribers.GetAll();
            return Ok(subscribers);
        }
    }
}