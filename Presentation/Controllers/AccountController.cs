using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Presentation.ViewModels;

namespace Presentation.Controllers {
    [ApiController]
    [Route ("api/[controller]")]
    public class AccountController : ControllerBase {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private IGenericRepository<TtcUser> _userGenericRepo;
        private IConfiguration _config;

        public AccountController (
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IGenericRepository<TtcUser> userGenericRepo,
            IConfiguration config) {
            _signInManager = signInManager;
            _userManager = userManager;
            _userGenericRepo = userGenericRepo;
            _config = config;
        }

        [HttpPost ("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register ([FromBody] TtcUserVM input) {
            if (ModelState.IsValid) {
                var user = new IdentityUser { UserName = input.Email, Email = input.Email };
                var result = await _userManager.CreateAsync (user, input.Password);
                if (result.Succeeded) {
                    if (input.Role != null) {
                        await _userManager.AddToRoleAsync (user, input.Role);
                    }

                    await _userGenericRepo.AddAsync (new TtcUserVM {
                        Email = input.Email,
                            FirstName = input.FirstName,
                            LastName = input.LastName
                    });
                    //later send a link for email confirmation 
                    //now just return an OKobjectResult after which we return a token
                    return Ok ("User Successfully created");
                } else {
                    return BadRequest (result.Errors);
                }
            }

            return BadRequest (ModelState);
        }

        [HttpPost ("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login ([FromBody] LoginModel loginDetails) {
            if (ModelState.IsValid) {
                var result = await _signInManager.PasswordSignInAsync (loginDetails.Email, loginDetails.Password, true, false);
                if (result.Succeeded) {
                    var key = _config.GetSection ("AppSettings:Token").Value;
                    string token = CreateToken (loginDetails.Email, key);
                    return Ok (new { token });
                }
                if (result.IsLockedOut) {
                    return BadRequest ("User account is locked out");
                } else {
                    ModelState.AddModelError ("description", "Invalid login attempt");
                    return BadRequest (ModelState);
                }
            }
            return BadRequest (ModelState);
        }

        [HttpPost ("logout")]

        public async Task<ActionResult> Logout () {
            await _signInManager.SignOutAsync ();
            return Ok ();
        }

        private string CreateToken (string email, string blobKey) {
            List<Claim> claims = new List<Claim> {
                new Claim (ClaimTypes.Name, email)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey (Encoding.ASCII
                .GetBytes (blobKey));

            SigningCredentials creds = new SigningCredentials (key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity (claims),
                Expires = DateTime.Now.AddDays (1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler ();
            SecurityToken token = tokenHandler.CreateToken (tokenDescriptor);

            return tokenHandler.WriteToken (token);
        }

        [HttpGet("user")]
        public IActionResult GetUser()
        {
            var user = HttpContext.User.Identity.Name;
            if (user != null)
            {
                var currentUser = _userGenericRepo.GetAll().FirstOrDefault(m => m.Email == user);

                var response = new UserResponse
                {
                    User = currentUser
                };

                return Ok(response);
            }

            return NotFound(null);
        }

    }

    public class UserResponse
    {
        public TtcUser User { get; set; }
    }
}