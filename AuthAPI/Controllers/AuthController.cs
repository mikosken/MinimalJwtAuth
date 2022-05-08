using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Authorize()] // Default action should be to authorize for access.
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        public AuthController(IConfiguration config, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _config = config;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
        }
    }
}