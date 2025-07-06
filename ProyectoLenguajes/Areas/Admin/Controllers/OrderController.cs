using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoLenguajes.Data;
using ProyectoLenguajes.Areas.Admin.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using ProyectoLenguajes.Utilities;

namespace ProyectoLenguajes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Utilities.StaticValues.Role_Admin)]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Order
        public async Task<IActionResult> Index(string clientId, int? statusId, DateTime? startDate, DateTime? endDate)
        {
            var ordersQuery = _dbContext.Orders
                .Include(o => o.Client)
                .Include(o => o.Status)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .AsQueryable();

            // Aplicar filtros si están presentes
            if (!string.IsNullOrEmpty(clientId))
                ordersQuery = ordersQuery.Where(o => o.ClientId == clientId);

            if (statusId.HasValue)
                ordersQuery = ordersQuery.Where(o => o.StatusId == statusId.Value);

            if (startDate.HasValue)
                ordersQuery = ordersQuery.Where(o => o.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                ordersQuery = ordersQuery.Where(o => o.CreatedAt <= endDate.Value);

            // Construir el ViewModel con los filtros y resultados
            var model = new OrderFilterVM
            {
                ClientId = clientId,
                StatusId = statusId,
                StartDate = startDate,
                EndDate = endDate,
                Clients = await _dbContext.Users
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id,
                        Text = u.UserName,

                    }).ToListAsync(),
                Statuses = await _dbContext.Status
                    .Where(s => s.Name != StaticValues.Status_Unconfirmed)
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    }).ToListAsync(),
                Orders = await ordersQuery.OrderByDescending(o => o.CreatedAt).ToListAsync()
            };

            return View(model);
        }

        // POST: /Order/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, int newStatusId)
        {
            var order = await _dbContext.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound();

            order.StatusId = newStatusId;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }

}
