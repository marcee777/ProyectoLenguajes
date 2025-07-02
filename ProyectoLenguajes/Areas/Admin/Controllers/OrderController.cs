using Microsoft.AspNetCore.Mvc;
using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models;
using ProyectoLenguajes.Utilities; // Para usar StaticValues
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoLenguajes.Areas.Admin.Controllers
{
    [Area("Admin")]
    // TODO: Add authorization attribute for admin role
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        // Allowed statuses from StaticValues
        private readonly List<string> AllowedStatuses = new List<string>
        {
            StaticValues.Status_OnTime,
            StaticValues.Status_OverTime,
            StaticValues.Status_Delayed,
            StaticValues.Status_Canceled,
            StaticValues.Status_Delivered
        };

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Admin/Order
        // Optional filters: clientId, startDate, endDate, status
        public IActionResult Index(string? clientId, DateTime? startDate, DateTime? endDate, string? status)
        {
            var orders = _unitOfWork.Order.GetAll(includeProperties: "OrderDetails.Dish,Client,Status").AsQueryable();

            if (!string.IsNullOrEmpty(clientId))
            {
                orders = orders.Where(o => o.ClientId == clientId);
            }

            if (startDate.HasValue)
            {
                orders = orders.Where(o => o.CreatedAt >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                orders = orders.Where(o => o.CreatedAt <= endDate.Value.Date.AddDays(1).AddTicks(-1));
            }

            if (!string.IsNullOrEmpty(status) && AllowedStatuses.Contains(status))
            {
                orders = orders.Where(o => o.Status.Name == status);
            }

            var filteredList = orders.OrderByDescending(o => o.CreatedAt).ToList();

            ViewBag.Statuses = AllowedStatuses;

            return View(filteredList);
        }

        // POST: Change order status
        [HttpPost]
        public IActionResult ChangeStatus(int orderId, string newStatus)
        {
            if (!AllowedStatuses.Contains(newStatus))
                return Json(new { success = false, message = "Invalid status" });

            var order = _unitOfWork.Order.Get(o => o.Id == orderId);
            if (order == null)
                return Json(new { success = false, message = "Order not found" });

            order.Status = newStatus;
            _unitOfWork.Order.Update(order);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Status updated successfully" });
        }
    }
}
