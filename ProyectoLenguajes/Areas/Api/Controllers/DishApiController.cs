using Microsoft.AspNetCore.Mvc;
using ProyectoLenguajes.Models.ApiModels;
using Microsoft.EntityFrameworkCore;
using ProyectoLenguajes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ProyectoLenguajes.Areas.Api.Controllers
{
    [Area("Api")]
    [Route("Api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DishApiController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public DishApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener lista de platos activos (solo nombre y precio), con búsqueda opcional
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var query = _context.Dishes
                .Where(d => d.IsActive);

            if (!string.IsNullOrEmpty(search))
            {
                var lowered = search.ToLower();
                query = query.Where(d => d.Name.ToLower().Contains(lowered));
            }

            var dishes = await query
                .OrderBy(d => d.Name)
                .Select(d => new DishListDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Price = d.Price
                })
                .ToListAsync();

            return Ok(dishes);
        }

        // Obtener detalles completos de un plato activo por id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dish = await _context.Dishes
                .Where(d => d.Id == id && d.IsActive)
                .Select(d => new DishDetailDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    Price = d.Price,
                    URLImage = d.URLImage
                })
                .FirstOrDefaultAsync();

            if (dish == null)
                return NotFound(new { Success = false, Message = "Dish not found or inactive" });

            return Ok(dish);
        }
    }
}
