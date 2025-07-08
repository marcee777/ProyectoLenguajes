using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoLenguajes.Data;
using ProyectoLenguajes.Models;
using ProyectoLenguajes.Models.ApiModels;
using ProyectoLenguajes.Utilities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ProyectoLenguajes.Areas.Api
{
    [Area("Api")]
    [Route("Api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    /**
     * Controlador API para la gestión del carrito y las órdenes de los clientes.
     * Permite obtener la orden activa, agregar o eliminar platos del carrito,
     * confirmar la orden y consultar información básica del usuario autenticado.
     * 
     * Todos los endpoints están protegidos por autenticación JWT.
     * 
     * @author: Melanie Arce C30634
     * @author: Carolina Rodríguez C36640
     * @author: Marcela Rojas C36975
     * @version: 07/07/25
     */
    public class OrderApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private static int? lastUndoneOrderId = null;

        public OrderApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener carrito actual (orden con estado "Unconfirmed")
        /**
         * Obtiene la orden activa (carrito) del cliente autenticado, es decir,
         * aquella con estado "Unconfirmed".
         * 
         * @return Un objeto OrderDto con los detalles de la orden activa, o null si no hay una.
         * 
         * Ruta: GET /Api/OrderApi/My-Active
         * Protegido por JWT.
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */

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
        /**
         * Agrega un plato al carrito del usuario autenticado. Si no hay una orden activa,
         * se crea una nueva. Si el plato ya estaba, se incrementa la cantidad.
         * 
         * @param DishId Id del plato a agregar
         * @param Amount Cantidad del plato a agregar
         * @return Mensaje de éxito o error en formato JSON
         * 
         * Ruta: POST /Api/OrderApi/Add-Item
         * Protegido por JWT.
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */
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


        /**
         * Elimina un ítem específico del carrito del usuario autenticado.
         * 
         * @param dishId Id del plato a eliminar
         * @return Mensaje de éxito o error si no se encuentra la orden o el ítem
         * 
         * Ruta: DELETE /Api/OrderApi/Remove-Item/{dishId}
         * Protegido por JWT.
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */

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

        /**
         * Confirma la orden activa (carrito) del cliente autenticado, cambiando su estado
         * de "Unconfirmed" a "OnTime", y registrando la fecha de creación.
         * 
         * @return Mensaje de éxito o error si no hay carrito o si está vacío
         * 
         * Ruta: POST /Api/OrderApi/Confirm
         * Protegido por JWT.
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */
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
            order.LastStatusChange = order.CreatedAt;

            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Order confirmed successfully!" });
        }

    }
}
