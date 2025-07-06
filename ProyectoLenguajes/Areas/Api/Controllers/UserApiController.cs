using Microsoft.AspNetCore.Mvc;
using ProyectoLenguajes.Models;
using Microsoft.AspNetCore.Identity;
using ProyectoLenguajes.Models.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ProyectoLenguajes.Areas.Api.Controllers
{
    [Area("Api")]
    [Route("Api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserApiController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserApiController(
            UserManager<IdentityUser> userManager,
            IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        }

        // GET: Api/UserApi/profile

        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
                return Unauthorized();

            var user = identityUser as ApplicationUser;
            if (user == null)
                return BadRequest("El usuario no es del tipo ApplicationUser.");

            var profile = new UserProfileDto
            {
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address
            };

            return Ok(profile);
        }

        // PUT: Api/UserApi/Profile
        [HttpPut("Profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
                return Unauthorized();

            var user = identityUser as ApplicationUser;
            if (user == null)
                return BadRequest("El usuario no es del tipo ApplicationUser.");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address ?? string.Empty;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return BadRequest(new
                {
                    Success = false,
                    Errors = updateResult.Errors
                });
            }

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                if (model.NewPassword.Length < 6)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Errors = new[] { new { Code = "PasswordTooShort", Description = "Password must be at least 6 characters long." } }
                    });
                }

                var validationResult = await _userManager.PasswordValidators[0]
                    .ValidateAsync(_userManager, user, model.NewPassword);

                if (!validationResult.Succeeded)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Errors = validationResult.Errors
                    });
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Errors = passwordResult.Errors
                    });
                }
            }

            return Ok(new
            {
                Success = true,
                Message = string.IsNullOrWhiteSpace(model.NewPassword)
                    ? "Profile updated successfully (no password change)."
                    : "Profile and password updated successfully."
            });
        }
    }
}