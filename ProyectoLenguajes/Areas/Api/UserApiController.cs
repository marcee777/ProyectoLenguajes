using Microsoft.AspNetCore.Mvc;
using ProyectoLenguajes.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using ProyectoLenguajes.Models.ApiModels;
using Microsoft.Extensions.DependencyInjection; // Para acceder a RoleManager dinámicamente

namespace ProyectoLenguajes.Areas.Api.Controllers
{
    [Area("Api")]
    [Route("Api/[controller]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserApiController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // Registro de usuario
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address ?? string.Empty
            };

            // Crear el usuario
            IdentityUser identityUser = user;
            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    Success = false,
                    Errors = result.Errors
                });
            }

            // Obtener el RoleManager desde el contenedor de servicios
            var roleManager = HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();

            // Crear rol "Customer" si no existe
            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                await roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            // Asignar rol "Customer"
            await _userManager.AddToRoleAsync(identityUser, "Customer");

            return Ok(new { Success = true, Message = "User registered and assigned to Customer role successfully" });
        }

        // Obtener datos del usuario (requiere autenticación)
        //[Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
                return Unauthorized();

            var user = identityUser as ApplicationUser;
            if (user == null)
                return BadRequest("User is not of type ApplicationUser.");

            var profile = new UserProfileDto
            {
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address
            };

            return Ok(profile);
        }

        // Actualizar datos del usuario (nombre, dirección y opcionalmente contraseña)
        //[Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
                return Unauthorized();

            var user = identityUser as ApplicationUser;
            if (user == null)
                return BadRequest("User is not of type ApplicationUser.");

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

            // Cambiar contraseña si se envió una nueva
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
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

            return Ok(new { Success = true, Message = "Profile updated successfully" });
        }
    }
}
