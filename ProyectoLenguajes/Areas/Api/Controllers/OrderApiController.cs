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

        // Obtener carrito actual (orden con estado "Unconfirmed")
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
                .Include(o => o.Client)
                .FirstOrDefaultAsync(o => o.ClientId == userId && o.Status.Name == StaticValues.Status_Unconfirmed);

            if (o == null) return Ok(null);

            var dto = new OrderDto
            {
                Id = o.Id,
                ClientName = $"{o.Client.FirstName} {o.Client.LastName}",
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

        // Agregar plato al carrito
        [HttpPost("Add-Item")]
        public async Task<IActionResult> AddItem([FromForm] int DishId, [FromForm] int Amount)
        {
            if (Amount <= 0)
                return BadRequest(new { Success = false, Message = "Amount must be greater than zero" });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID is null or empty." });

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return BadRequest(new { message = "User does not exist in database.", userId });

            var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == DishId && d.IsActive);
            if (dish == null)
                return BadRequest(new { Success = false, Message = "Dish not found or inactive" });

            var status = await _context.Status.FirstOrDefaultAsync(s => s.Name == StaticValues.Status_Unconfirmed);
            if (status == null)
                return BadRequest(new { Success = false, Message = "Unconfirmed status not found" });

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

            return Ok(new { Success = true, Message = "Item added to cart" });
        }

        // Eliminar ítem del carrito
        [HttpDelete("Remove-Item/{dishId}")]
        public async Task<IActionResult> RemoveItem(int dishId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var status = await _context.Status.FirstOrDefaultAsync(s => s.Name == StaticValues.Status_Unconfirmed);
            if (status == null)
                return BadRequest(new { Success = false, Message = "Unconfirmed status not found" });

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.ClientId == userId && o.StatusId == status.Id);

            if (order == null)
                return NotFound(new { Success = false, Message = "No active cart" });

            var item = order.OrderDetails.FirstOrDefault(od => od.DishId == dishId);
            if (item == null)
                return NotFound(new { Success = false, Message = "Item not found" });

            order.OrderDetails.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Item removed from cart" });
        }

        // Confirmar carrito
        [HttpPost("Confirm")]
        public async Task<IActionResult> ConfirmMyCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var unconfirmedStatus = await _context.Status
                .FirstOrDefaultAsync(s => s.Name == StaticValues.Status_Unconfirmed);

            var onTimeStatus = await _context.Status
                .FirstOrDefaultAsync(s => s.Name == StaticValues.Status_OnTime);

            if (unconfirmedStatus == null || onTimeStatus == null)
                return BadRequest(new { Success = false, Message = "Statuses not properly configured." });

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.ClientId == userId && o.StatusId == unconfirmedStatus.Id);

            if (order == null)
                return NotFound(new { Success = false, Message = "No unconfirmed cart to confirm." });

            if (order.OrderDetails == null || !order.OrderDetails.Any())
                return BadRequest(new { Success = false, Message = "Cannot confirm an empty cart." });

            order.StatusId = onTimeStatus.Id;
            order.CreatedAt = DateTime.Now; // ⚠️ Reiniciar CreatedAt al confirmar
            order.LastStatusChange = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Order confirmed successfully!" });
        }


        // Pruebas
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
