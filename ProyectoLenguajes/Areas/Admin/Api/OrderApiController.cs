using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoLenguajes.Data;
using ProyectoLenguajes.Models;
using ProyectoLenguajes.Utilities;

namespace ProyectoLenguajes.Areas.Admin.Api
{
    [Area("Admin")]
    [Route("Admin/Api/[controller]")]
    [ApiController]
    public class OrderApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private static int? lastUndoneOrderId = null;

        public OrderApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/api/OrderApi
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? clientId, [FromQuery] int? statusId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var query = _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Status)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .AsQueryable();

            if (!string.IsNullOrEmpty(clientId))
                query = query.Where(o => o.ClientId == clientId);

            if (statusId.HasValue)
                query = query.Where(o => o.StatusId == statusId);

            if (startDate.HasValue)
                query = query.Where(o => o.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.CreatedAt <= endDate.Value);

            var orders = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();

            return Ok(orders);
        }

        // GET: Admin/api/OrderApi/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveOrders()
        {
            var activeOrders = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Status)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .Where(o => o.Status.Name != StaticValues.Status_Canceled &&
                            o.Status.Name != StaticValues.Status_Delivered)
                .OrderBy(o => o.CreatedAt)
                .ToListAsync();

            return Ok(activeOrders);
        }

        // GET: Admin/api/OrderApi/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Status)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { Success = false, Message = "Order not found" });
            }

            return Ok(order);
        }

        // POST: Admin/api/OrderApi/updateStatus
        [HttpPost("updateStatus")]
        public async Task<IActionResult> UpdateStatus([FromForm] int orderId, [FromForm] int newStatusId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound(new { Success = false, Message = "Order not found" });
            }

            lastUndoneOrderId = order.Id;
            order.StatusId = newStatusId;
            order.LastStatusChange = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Order status updated" });
        }

        // POST: Admin/api/OrderApi/undo
        [HttpPost("undo")]
        public async Task<IActionResult> UndoLastChange()
        {
            if (lastUndoneOrderId == null)
            {
                return BadRequest(new { Success = false, Message = "No order to undo" });
            }

            var order = await _context.Orders.FindAsync(lastUndoneOrderId.Value);
            if (order == null)
            {
                return NotFound(new { Success = false, Message = "Order not found" });
            }

            var defaultStatus = await _context.Status.FirstOrDefaultAsync(s => s.Name == StaticValues.Status_OnTime);
            if (defaultStatus == null)
            {
                return BadRequest(new { Success = false, Message = "Default status not found" });
            }

            order.StatusId = defaultStatus.Id;
            order.LastStatusChange = DateTime.Now;

            await _context.SaveChangesAsync();
            lastUndoneOrderId = null;

            return Ok(new { Success = true, Message = "Order status reverted" });
        }
    }
}
