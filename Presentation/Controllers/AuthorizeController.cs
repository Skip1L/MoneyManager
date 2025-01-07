using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Constants;
using Domain.Entities;
using DTOs.AuthorizationDTOs;
using FluentValidation;
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
    public class AuthorizeController(ILogger<AuthorizeController> logger, UserManager<User> userManager, SignInManager<User> signInManager, IValidator<SignUpDTO> validator) : ControllerBase
    {
        private readonly ILogger<AuthorizeController> _logger = logger;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly IValidator<SignUpDTO> _signUpDTOValidator = validator;

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
            var validationResult = await _signUpDTOValidator.ValidateAsync(signUpDTO);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    _logger.LogWarning($"Validation failed for {error.PropertyName}: {error.ErrorMessage}");
                }

                return BadRequest(validationResult.Errors.FirstOrDefault().ErrorMessage);
            }

            _logger.LogInformation("SignUpDTO validated successfully.");

            var existingUser = await _userManager.FindByEmailAsync(signUpDTO.Email);
            if (existingUser != null)
            {
                return Conflict(new { message = "A user with this email already exists." });
            }

            var existingUsername = await _userManager.FindByNameAsync(signUpDTO.Username);
            if (existingUsername != null)
            {
                return Conflict(new { message = "A user with this username already exists." });
            }

            var user = new User
            {
                UserName = signUpDTO.Username,
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
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            await _userManager.AddToRoleAsync(user, Roles.DEFAULT_USER);

            return Ok("User registered successfully!");
        }

        [HttpPost("admin/setLockoutEnd")]
        [Authorize(Roles = Roles.ADMINISTRATOR)]
        public async Task<IActionResult> SetLockoutEnd([FromBody] SetLockoutEndDTO setLockoutEndDTO)
        {
            var user = await _userManager.FindByIdAsync(setLockoutEndDTO.UserId.ToString());

            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", setLockoutEndDTO.UserId);
                return NotFound("User not found.");
            }

            user.LockoutEnd = setLockoutEndDTO.LockoutEnd;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogWarning($"Error updating LockoutEnd for user: {setLockoutEndDTO.UserId}");
                return BadRequest("Failed to update LockoutEnd.");
            }

            _logger.LogInformation($"Successfully updated LockoutEnd for user: {setLockoutEndDTO.UserId}");
            return Ok("LockoutEnd updated successfully.");
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

            var envKey = "sjgienghs;vcsfrtuifs1)d56fdsaw67";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(envKey));
            var exp = 7200;

            var jwt = new JwtSecurityToken(
               issuer: "MoneyTrackerIssuer",
               audience: "MoneyTrackerHost",
               claims: claims,
               expires: DateTime.UtcNow.AddSeconds(exp),
               notBefore: DateTime.UtcNow,
               signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
