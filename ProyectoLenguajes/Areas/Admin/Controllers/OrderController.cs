using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoLenguajes.Areas.Admin.Views.ViewModel;
using ProyectoLenguajes.Data.Repository.Interfaces;

namespace ProyectoLenguajes.Areas.Admin.Controllers
{
    [Area("Admin")]
    // [Authorize(Roles = StaticValues.Role_Admin)] // puedes habilitarlo si quieres
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Admin/Order
        public IActionResult Index(string? clientId, int? statusId, DateTime? startDate, DateTime? endDate)
        {
            var ordersQuery = _unitOfWork.Order
                .GetAll(includeProperties: "OrderDetails.Dish,Client,Status")
                .AsQueryable();

            if (!string.IsNullOrEmpty(clientId))
                ordersQuery = ordersQuery.Where(o => o.ClientId == clientId);

            if (statusId.HasValue)
                ordersQuery = ordersQuery.Where(o => o.StatusId == statusId.Value);

            if (startDate.HasValue)
                ordersQuery = ordersQuery.Where(o => o.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                ordersQuery = ordersQuery.Where(o => o.CreatedAt <= endDate.Value);

            var orders = ordersQuery
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderListItemVM
                {
                    OrderId = o.Id,
                    ClientName = o.Client.UserName,
                    StatusName = o.Status.Name,
                    CreatedAt = o.CreatedAt,
                    Items = o.OrderDetails.Select(od => new OrderDishItemVM
                    {
                        DishName = od.Dish.Name,
                        Amount = od.Amount
                    }).ToList()
                })
                .ToList();

            var vm = new OrderIndexVM
            {
                Orders = orders,
                ClientList = _unitOfWork.ApplicationUsers.GetAll()
                    .Select(u => new SelectListItem { Text = u.UserName, Value = u.Id })
                    .ToList(),
                StatusList = _unitOfWork.Status.GetAll()
                    .Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() })
                    .ToList(),
                SelectedClientId = clientId,
                SelectedStatusId = statusId,
                StartDate = startDate,
                EndDate = endDate
            };

            return View(vm);
        }

        // POST: Change order status
        [HttpPost]
        public IActionResult ChangeStatus(int orderId, int newStatusId)
        {
            var statusExists = _unitOfWork.Status.GetAll().Any(s => s.Id == newStatusId);
            if (!statusExists)
                return Json(new { success = false, message = "Invalid status" });

            var order = _unitOfWork.Order.Get(o => o.Id == orderId);
            if (order == null)
                return Json(new { success = false, message = "Order not found" });

            order.StatusId = newStatusId;
            _unitOfWork.Order.Update(order);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Status updated successfully" });
        }
    }
}
