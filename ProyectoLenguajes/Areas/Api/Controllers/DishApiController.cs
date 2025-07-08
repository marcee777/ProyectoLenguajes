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

    /**
     * Controlador API para la gestión y consulta de platos activos.
     * Proporciona endpoints para obtener la lista de platos activos (nombre y precio) y
     * para obtener los detalles completos de un plato específico.
     * 
     * Está protegido mediante autenticación JWT para garantizar acceso autorizado.
     * 
     * @autor: Melanie Arce C30634
     * @autor: Carolina Rodríguez C36640
     * @autor: Marcela Rojas C36975
     * @version: 07/07/25
     */
    public class DishApiController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public DishApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener lista de platos activos (solo nombre y precio), con búsqueda opcional
        /**
         * Obtiene la lista de platos activos, permitiendo filtrar por nombre mediante búsqueda opcional.
         * 
         * @param search Parámetro opcional para filtrar platos cuyo nombre contenga esta cadena (case-insensitive)
         * @return Lista de objetos DishListDto con Id, Nombre y Precio de los platos activos que cumplen el filtro
         * 
         * Método accesible vía GET en /Api/DishApi
         * 
         * @autor: Melanie Arce C30634
         * @autor: Carolina Rodríguez C36640
         * @autor: Marcela Rojas C36975
         * @version: 07/07/25
         */

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
        /**
         * Obtiene los detalles completos de un plato activo específico por su Id.
         * 
         * @param id Identificador del plato a consultar
         * @return Detalle del plato en formato DishDetailDto o NotFound si no existe o está inactivo
         * 
         * Método accesible vía GET en /Api/DishApi/{id}
         * 
         * @autor: Melanie Arce C30634
         * @autor: Carolina Rodríguez C36640
         * @autor: Marcela Rojas C36975
         * @version: 07/07/25
         */

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
