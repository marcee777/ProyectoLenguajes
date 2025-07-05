using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    public class AuthApiController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthApiController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // POST: Api/AuthApi/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Aquí podrías guardar FirstName, LastName y Address en otra tabla si quieres.

            return Ok(new { Success = true, Message = "User registered" });
        }

        // POST: Api/AuthApi/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { Success = false, Message = "Invalid credentials" });

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new { Success = false, Message = "Invalid credentials" });

            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new[]
            {
        // Usamos el ID del usuario como el "subject" principal del token (sub)
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),

        // También agregamos el Id en ClaimTypes.NameIdentifier (opcional pero común)
        new Claim(ClaimTypes.NameIdentifier, user.Id),

        // Correo en otro claim para tenerlo disponible si se necesita
        new Claim(JwtRegisteredClaimNames.Email, user.Email),

        // Nombre de usuario
        new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),

        // Jti para identificación única del token
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
