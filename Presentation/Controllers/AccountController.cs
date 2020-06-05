using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.ViewModels;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

    
        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("create")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody]TtcUserVM input)
        {
            if(ModelState.IsValid)
            {
                var user = new IdentityUser {UserName = input.Email, Email = input.Email };
                var result = await _userManager.CreateAsync(user, input.Password);
                if(result.Succeeded)
                {
                    if(input.Role != null)
                    {
                        await _userManager.AddToRoleAsync(user, input.Role);
                    }  
                    //later send a link for email confirmation 
                    //now just return an OKobjectResult after which we return a token
                    return Ok("User Successfully created");
                }
                else 
                {
                    return BadRequest(result.Errors);
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody]LoginModel loginDetails)
        {
            if(ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginDetails.Email, loginDetails.Password, true, false);
                if(result.Succeeded)
                {
                    // return a jwt token later 
                    return Ok("User logged in");
                }
                if(result.IsLockedOut)
                {
                    return BadRequest("User account is locked out");
                }
                else{
                    ModelState.AddModelError(string.Empty, "Invalid login attempt");
                    return BadRequest(ModelState);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}
