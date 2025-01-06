using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Constants;
using Domain.Entities;
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

            var isAdministrator = !roles.Contains(Roles.ADMINISTRATOR);

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
