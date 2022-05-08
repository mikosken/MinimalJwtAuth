using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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

        // POST api/auth/create-role?roleName=xyz
        [HttpPost("create-role")]
        [Authorize(Policy = "Admins")] // Only admins are allowed to create roles.
        public async Task<IActionResult> CreateRole([FromQuery] string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

                if (!result.Succeeded) {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("User registration", error.Description);
                    }
                    // 400, BadRequest.
                    return StatusCode(400, ModelState);
                }
                // 201, Created.
                return StatusCode(201);
            }

        return BadRequest("Rollen finns redan");

        }

        // POST: api/auth/register
        [HttpPost("register")]
        [AllowAnonymous] // Anyone can try to register.
        public async Task<ActionResult<UserViewModel>> RegisterUser(RegisterUserPostViewModel model)
        {
            // Attempt to create and save a new IdentityUser using supplied information.
            var user = new IdentityUser
            {
                // RegisterUserPostViewModel has [Required] attributes so we
                // know that they're not null, hence .Email!. 
                // We set username to email, if we wanted a different username
                // we could add that to ViewModel and modify below.
                // Don't attempt to set password here, use UserManager.
                Email = model.Email!.ToLower(),
                UserName = model.Email!.ToLower()
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            // Verify success.
            if (result.Succeeded)
            {
                // Create claims if specified in model.
                // Example:
                // if (model.IsAdmin)
                // {
                //     await _userManager.AddClaimAsync(user, new Claim("Admin", "true"));
                //     await _userManager.AddToRoleAsync(user, "Administrators");
                // }

                // All users get the claim "User", username, and email.
                // Could specify other claims, or create a different API method
                // to set/modify claims in db.
                await _userManager.AddClaimAsync(user, new Claim("User", "true"));
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, user.UserName));
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.Email));

                // Create and return UserViewModel
                var userData = new UserViewModel
                {
                    UserName = user.UserName,
                    Token = await CreateJwtToken(user)
                };
                // 201, Created.
                return StatusCode(201, userData);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("User registration", error.Description);
                }
                // 500, InternalServerError.
                return StatusCode(500, ModelState);
            }
        }

        // POST: api/login
        [HttpPost("login")]
        [AllowAnonymous] // We shouldn't need to be logged in to login...
        public async Task<ActionResult<UserViewModel>> Login(LoginPostViewModel model)
        {
            // Verify that the user exists
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user is null)
                return Unauthorized("Incorrect username or password.");

            // Try logging in.
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Incorrect username or password.");

            // If successfully logged in, return a User ViewModel object.
            var userVM = new UserViewModel
            {
                UserName = user.UserName,
                Token = await CreateJwtToken(user)
            };

            return Ok(userVM);
        }

        private async Task<string> CreateJwtToken(IdentityUser user) {
            // Helper method to create a JWT (JSON Web Token) for specified user.
            // This method could be extended to take options for what claims
            // to include, change validFrom, etc.
            var signingKey = Encoding.UTF8.GetBytes(_config.GetValue<string>("JwtSigningKey"));
            var validForSeconds = _config.GetValue<double>("JwtValidForSeconds");
            var validFrom = DateTime.Now; // Could be set in the future.

            // Get user claims and roles from identity db.
            var userClaims = (await _userManager.GetClaimsAsync(user)).ToList();
            var roles = await _userManager.GetRolesAsync(user);
            // Convert roles into role-claims and add to user claims.
            userClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            // Create the token.
            var jwt = new JwtSecurityToken(
                claims: userClaims,
                // notBefore: When the token becomes valid, could be in the future.
                notBefore: validFrom,
                // For how long should the token be valid after activation?
                expires: validFrom.AddSeconds(validForSeconds),
                // signingCredentials for signing the JWT so it can't be faked.
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(signingKey),
                    SecurityAlgorithms.HmacSha512Signature
                )
            );
            // JwtSecurityTokenHandler has a neat method to convert JWT to string.
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}