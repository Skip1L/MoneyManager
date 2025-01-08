using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
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
    public class AuthorizeController(ILogger<AuthorizeController> logger, UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper) : ControllerBase
    {
        private readonly ILogger<AuthorizeController> _logger = logger;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly IMapper _mapper = mapper;

        [HttpPost("signIn")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] SignInDTO signInDTO)
        {
            var user = await _userManager.FindByNameAsync(signInDTO.UserName);

            if (user == null)
            {
                return BadRequest();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var isAdministrator = roles.Contains(Roles.Administrator);

            var signInResult = await _signInManager.PasswordSignInAsync(user, signInDTO.Password, false, !isAdministrator);

            if (signInResult.Succeeded)
            {
                await _userManager.ResetAccessFailedCountAsync(user);

                var profile = await GetUserProfile(user, [.. roles]);

                return Ok(profile);
            }

            _logger.LogWarning($"Incorrect password for user : {signInDTO.UserName}");

            return BadRequest();
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
                || signUpDTO.DateOfBirth > DateTime.UtcNow)
            {
                return BadRequest();
            }

            var existingUser = await _userManager.FindByEmailAsync(signUpDTO.Email);
            if (existingUser != null)
            {
                return Conflict();
            }

            if (signUpDTO.Roles.Contains(Roles.Administrator) 
                && HttpContext.User?.IsInRole(Roles.Administrator) != true)
            {
                return Forbid();
            }

            var user = _mapper.Map<User>(signUpDTO);

            var result = await _userManager.CreateAsync(user, signUpDTO.Password);

            if (!result.Succeeded)
            {
                _logger.LogWarning($"User are not created : {result.Errors.Select(e => e.Description)}");
                return BadRequest();
            }

            await _userManager.AddToRolesAsync(user, signUpDTO.Roles);

            return Ok();
        }

        [HttpPut("unlockUser")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> UnlockUser([FromBody] string userToUnlock)
        {
            var user = await _userManager.FindByNameAsync(userToUnlock);

            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userToUnlock);
                return NotFound();
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null);
            await _userManager.ResetAccessFailedCountAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogWarning($"Error updating LockoutEnd for user: {user.UserName}. {result.Errors.Select(e => e.Description)}");
                return BadRequest();
            }

            return Ok();
        }

        private async Task<SuccessSignInDTO> GetUserProfile(User user, List<string> roles)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            roles.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));

            var signedInUserDto = _mapper.Map<SuccessSignInDTO>(user);
            signedInUserDto.Roles = roles;
            signedInUserDto.Token = GetToken(user, [.. claims]);

            return signedInUserDto;
        }

        private static string GetToken(IdentityUser<Guid> user, List<Claim> claims)
        {
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));

            var envKey =  AuthOptionsHelper.GetSecretKey();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(envKey));
            var exp = AuthOptionsHelper.GetTokenExpirationTime();

            var jwt = new JwtSecurityToken(
               issuer: AuthOptionsHelper.GetIssuer(),
               audience: AuthOptionsHelper.GetAudience(),
               claims: claims,
               expires: DateTime.UtcNow.AddSeconds(exp),
               notBefore: DateTime.UtcNow,
               signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
