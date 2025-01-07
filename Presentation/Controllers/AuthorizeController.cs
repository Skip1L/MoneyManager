using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Constants;
using Domain.Entities;
using Domain.Helpers;
using DTOs.AuthorizationDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class AuthorizeController(ILogger<AuthorizeController> logger, UserManager<User> userManager, SignInManager<User> signInManager) : ControllerBase
    {
        private readonly ILogger<AuthorizeController> _logger = logger;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;

        [HttpPost("signIn")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] SignInDTO signInDTO)
        {
            var user = await _userManager.FindByNameAsync(signInDTO.UserName);

            if (user == null)
            {
                return BadRequest("Incorrect email or password!");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var isAdministrator = roles.Contains(Roles.ADMINISTRATOR);

            var signInResult = await _signInManager.PasswordSignInAsync(user, signInDTO.Password, false, !isAdministrator);

            if (signInResult.Succeeded)
            {
                await _userManager.ResetAccessFailedCountAsync(user);

                var profile = await GetUserProfile(user, [.. roles]);

                return Ok(profile);
            }

            _logger.LogWarning($"Incorrect password for user : {signInDTO.UserName}");

            return BadRequest("Incorrect email or password!");
        }

        [HttpPost("signUp")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] SignUpDTO signUpDTO)
        {
            if (string.IsNullOrEmpty(signUpDTO.FirstName)
                || string.IsNullOrEmpty(signUpDTO.LastName)
                || !PhoneNumberHelper.IsPhoneNumberValid(signUpDTO.PhoneNumber)
                || !EmailHelper.IsEmailValid(signUpDTO.Email)
                || !RolesHelper.IsRolesValid(signUpDTO.Roles)
                || !PasswordHelper.IsPasswordValid(signUpDTO.Password)
                || !DateOfBirthHelper.IsDateOfBirthValid(signUpDTO.DateOfBirth))
            {
                return BadRequest("An error occured while validating inputs");
            }

            var existingUser = await _userManager.FindByEmailAsync(signUpDTO.Email);
            if (existingUser != null)
            {
                return Conflict("A user with this email already exists.");
            }

            if (signUpDTO.Roles.Contains("ADMINISTRATOR"))
            {
                var currentUser = HttpContext.User;
                if (currentUser.Identity.IsAuthenticated)
                {
                    if (!currentUser.IsInRole("ADMINISTRATOR"))
                    {
                        return Forbid("Only admins can create new admin accounts.");
                    }
                }
                else
                {
                    return Unauthorized("You must be authorized to create an admin account.");
                }
            }

            var user = new User
            {
                UserName = signUpDTO.Email,
                Email = signUpDTO.Email,
                FirstName = signUpDTO.FirstName,
                LastName = signUpDTO.LastName,
                DateOfBirth = signUpDTO.DateOfBirth,
                PhoneNumber = signUpDTO.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, signUpDTO.Password);

            if (!result.Succeeded)
            {
                _logger.LogWarning($"User are not created : {result.Errors.Select(e => e.Description)}");
                return BadRequest("User are not created, because of unknown error");
            }

            await _userManager.AddToRolesAsync(user, signUpDTO.Roles);

            return Ok();
        }

        [HttpPut("unlockUser")]
        [Authorize(Roles = Roles.ADMINISTRATOR)]
        public async Task<IActionResult> UnlockUser([FromBody] string userToUnlock)
        {
            var user = await _userManager.FindByNameAsync(userToUnlock);

            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userToUnlock);
                return NotFound("User not found.");
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null);
            await _userManager.ResetAccessFailedCountAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogWarning($"Error updating LockoutEnd for user: {user.UserName}. {result.Errors.Select(e => e.Description)}");
                return BadRequest("Failed to update LockoutEnd.");
            }

            return Ok();
        }

        private async Task<SuccessSignInDTO> GetUserProfile(User user, List<string> roles)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            roles.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));

            return new SuccessSignInDTO
            {
                Created = user.CreatedAt,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                PhoneNumber = user.PhoneNumber,
                Roles = roles,
                Token = GetToken(user, [.. claims])
            };
        }

        private static string GetToken(IdentityUser<Guid> user, List<Claim> claims)
        {
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));

            var envKey = Environment.GetEnvironmentVariable("ISSUER_KEY");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(envKey));
            var exp = 7200;

            var jwt = new JwtSecurityToken(
               issuer: Environment.GetEnvironmentVariable("ISSUER"),
               audience: Environment.GetEnvironmentVariable("AUDIENCE"),
               claims: claims,
               expires: DateTime.UtcNow.AddSeconds(exp),
               notBefore: DateTime.UtcNow,
               signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
