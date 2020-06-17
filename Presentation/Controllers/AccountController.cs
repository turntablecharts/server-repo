using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Presentation.ViewModels;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITtcUserRepo _ttcUserRepo;
        private IConfiguration _config;


        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, 
             ITtcUserRepo ttcUserRepo,
             IConfiguration config)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _ttcUserRepo = ttcUserRepo;
            _config = config;
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

                    await _ttcUserRepo.Add(new TtcUserVM{Email = input.Email, 
                        FirstName = input.FirstName, 
                        LastName = input.LastName});
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
                    return Ok("user loggd in");
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
        
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        
    }
}
