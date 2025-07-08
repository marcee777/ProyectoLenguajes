using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProyectoLenguajes.Models;
using ProyectoLenguajes.Models.ApiModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProyectoLenguajes.Areas.Api.Controllers
{
    [Area("Api")]
    [Route("Api/[controller]")]
    [ApiController]

    /**
     * Controlador API para gestionar la autenticación de usuarios en la aplicación.
     * Permite registrar nuevos usuarios, iniciar sesión y generar tokens JWT para autenticación segura.
     * 
     * Utiliza ASP.NET Identity para la gestión de usuarios, roles y validaciones.
     * El controlador está en el área "Api" y expone endpoints REST para registro y login.
     * 
     * @author: Melanie Arce C30634
     * @author: Carolina Rodríguez C36640
     * @author: Marcela Rojas C36975
     * @version: 07/07/25
     */
    public class AuthApiController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        /**
         * Constructor del controlador AuthApiController
         * Inicializa los servicios necesarios para la gestión de usuarios, roles, inicio de sesión y configuración.
         * 
         * @param userManager Servicio para administrar usuarios
         * @param signInManager Servicio para gestionar inicio de sesión
         * @param roleManager Servicio para administrar roles
         * @param configuration Servicio para acceder a configuraciones de la aplicación, como parámetros JWT
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */
        public AuthApiController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        // POST: Api/AuthApi/Register

        /**
         * Endpoint para registrar un nuevo usuario cliente.
         * Valida el modelo recibido, verifica si el email ya está registrado,
         * crea el usuario y lo asigna al rol "Customer".
         * 
         * @param model DTO con los datos necesarios para registrar un usuario (Email, Password, Nombre, Apellido, Dirección)
         * @return Respuesta HTTP con mensaje de éxito o error según el resultado del registro
         * 
         * Método accesible vía POST en /Api/AuthApi/Register
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return Conflict(new { message = "Email is already registered." });

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return BadRequest(ModelState);
            }

            var roleExists = await _roleManager.RoleExistsAsync("Customer");
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            }
            await _userManager.AddToRoleAsync(user, "Customer");

            return Ok(new { message = "User registered successfully." });
        }

        // POST: Api/AuthApi/Login

        /**
         * Endpoint para autenticación de usuarios.
         * Valida las credenciales recibidas, verifica bloqueo, rol y genera un token JWT si la autenticación es exitosa.
         * 
         * @param model DTO con las credenciales para login (Email y Password)
         * @return Respuesta HTTP con token JWT para autenticación o mensaje de error en caso de falla
         * 
         * Método accesible vía POST en /Api/AuthApi/Login
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdentity = await _userManager.FindByEmailAsync(model.Email);
            var user = userIdentity as ApplicationUser;
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

            // Chequeo si está bloqueado
            if (await _userManager.IsLockedOutAsync(user))
            {
                return BadRequest(new { message = "Your account is locked. Please contact support." });
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
            if (!signInResult.Succeeded)
                return Unauthorized(new { message = "Invalid email or password." });

            // Aquí se valida el rol
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Customer"))
            {
                return Unauthorized(new { message = "You are not authorized to log in here. Only customers are allowed." });
            }

            var token = GenerateJwtToken(user, roles);

            return Ok(new { token });
        }

        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id), 
                new Claim(ClaimTypes.NameIdentifier, user.Id),       
                new Claim(ClaimTypes.Email, user.Email),             
                new Claim(ClaimTypes.Name, user.Email)               
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
