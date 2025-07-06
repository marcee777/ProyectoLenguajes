using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoLenguajes.Data;
using ProyectoLenguajes.Models;
using ProyectoLenguajes.Models.ApiModels;
using ProyectoLenguajes.Utilities;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ProyectoLenguajes.Areas.Api
{
    [Area("Api")]
    [Route("Api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private static int? lastUndoneOrderId = null;

        public OrderApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // CLIENTE: Obtener carrito actual (orden con estado "OnTime")

        // hace falta probar esto

        [HttpGet("My-Active")]
        public async Task<IActionResult> GetMyActiveOrder()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var o = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .Include(o => o.Status)
                .Include(o => o.Client) // 👈 Incluir los datos del cliente
                .FirstOrDefaultAsync(o => o.ClientId == userId && o.Status.Name == StaticValues.Status_OnTime);

            if (o == null) return Ok(null);

            var dto = new OrderDto
            {
                Id = o.Id,
                ClientName = $"{o.Client.FirstName} {o.Client.LastName}", // 👈 Usar el nombre real
                CreatedAt = o.CreatedAt,
                LastStatusChange = o.LastStatusChange,
                Status = new StatusDto
                {
                    Id = o.Status.Id,
                    Name = o.Status.Name
                },
                OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
                {
                    DishId = od.DishId,
                    DishName = od.Dish.Name,
                    DishPrice = od.Dish.Price,
                    Amount = od.Amount
                }).ToList()
            };

            return Ok(dto);
        }
        // hace falta probar esto


        // CLIENTE: Agregar plato al carrito

        [HttpPost("Add-Item")]

        public async Task<IActionResult> AddItem([FromForm] int DishId, [FromForm] int Amount)
        {
            if (Amount <= 0)
                return BadRequest(new { Success = false, Message = "Amount must be greater than zero" });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID is null or empty." });

            // Verificar si el usuario existe por su ID
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return BadRequest(new { message = "User does not exist in database.", userId });

            // Validar que el plato exista y esté activo (si tienes esa propiedad)
            var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == DishId && d.IsActive);
            if (dish == null)
                return BadRequest(new { Success = false, Message = "Dish not found or inactive" });


            var status = await _context.Status.FirstOrDefaultAsync(s => s.Name == StaticValues.Status_OnTime);
            if (status == null)
                return BadRequest(new { Success = false, Message = "Status not found" });

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.ClientId == userId && o.StatusId == status.Id);

            if (order == null)
            {
                order = new Order
                {
                    ClientId = userId,
                    StatusId = status.Id,
                    CreatedAt = DateTime.Now,
                    LastStatusChange = DateTime.Now,
                    OrderDetails = new List<OrderDetail>()
                };
                _context.Orders.Add(order);
            }

            var existingDetail = order.OrderDetails.FirstOrDefault(od => od.DishId == DishId);
            if (existingDetail != null)
                existingDetail.Amount += Amount;
            else
                order.OrderDetails.Add(new OrderDetail
                {
                    DishId = DishId,
                    Amount = Amount
                });

            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Item added to order" });
        }

        // CLIENTE: Eliminar ítem del carrito
        
        [HttpDelete("Remove-Item/{dishId}")]
        public async Task<IActionResult> RemoveItem(int dishId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var status = await _context.Status.FirstOrDefaultAsync(s => s.Name == StaticValues.Status_OnTime);
            if (status == null)
                return BadRequest(new { Success = false, Message = "Status not found" });

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.ClientId == userId && o.StatusId == status.Id);

            if (order == null)
                return NotFound(new { Success = false, Message = "No active order" });

            var item = order.OrderDetails.FirstOrDefault(od => od.DishId == dishId);
            if (item == null)
                return NotFound(new { Success = false, Message = "Item not found" });

            order.OrderDetails.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Item removed" });
        }

        
        //metodo para pruebas, para ver el id
        
        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var name = User.Identity?.Name;

            return Ok(new
            {
                userId,
                email,
                name
            });
        }
    }
}
