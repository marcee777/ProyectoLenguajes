using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models.ViewModels;
using ProyectoLenguajes.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoLenguajes.Areas.Kitchen.Controllers
{
    [Area("Kitchen")]
    //[Authorize(Roles = StaticValues.Role_Kitchen)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int MaxOrdersToShow = 7;

        // Claves de sesión para guardar último pedido entregado y estado previo
        private const string SessionLastDeliveredOrderId = "_LastDeliveredOrderId";
        private const string SessionLastDeliveredOrderPrevStatusId = "_LastDeliveredOrderPrevStatusId";

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Mostrar pedidos activos (cola)
        public IActionResult Index()
        {
            var allActiveOrders = _unitOfWork.Order
                .GetAll(includeProperties: "Client,OrderDetails.Dish,Status")
                .Where(o => o.Status.Name != StaticValues.Status_Canceled &&
                            o.Status.Name != StaticValues.Status_Delivered)
                .OrderBy(o => o.CreatedAt)
                .ToList();

            // Tomar máximo MaxOrdersToShow
            var ordersToShow = allActiveOrders.Take(MaxOrdersToShow).ToList();

            var orderVMs = ordersToShow.Select(o => new OrderVM
            {
                Order = o,
                OrderDetails = o.OrderDetails.ToList()
            }).ToList();

            // Indicar si hay más pedidos pendientes que los mostrados
            ViewBag.HasMoreOrders = allActiveOrders.Count > MaxOrdersToShow;

            // Indicar el id de la última orden entregada en sesión para "deshacer"
            ViewBag.LastDeliveredOrderId = HttpContext.Session.GetInt32(SessionLastDeliveredOrderId);

            return View(orderVMs);
        }

        // POST: Marcar como entregado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAsDelivered(int id)
        {
            var order = _unitOfWork.Order.Get(o => o.Id == id, includeProperties: "Status");
            if (order == null)
                return NotFound();

            var deliveredStatus = _unitOfWork.Status.Get(s => s.Name == StaticValues.Status_Delivered);
            if (deliveredStatus == null)
                return BadRequest("Delivered status not found.");

            // Guardar en sesión el Id del pedido y el estado previo antes de cambiarlo
            HttpContext.Session.SetInt32(SessionLastDeliveredOrderId, order.Id);
            HttpContext.Session.SetInt32(SessionLastDeliveredOrderPrevStatusId, order.StatusId);

            order.StatusId = deliveredStatus.Id;
            order.LastStatusChange = DateTime.Now;

            _unitOfWork.Order.Update(order);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        // POST: Deshacer último cambio a entregado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UndoLastDelivered()
        {
            var lastDeliveredOrderId = HttpContext.Session.GetInt32(SessionLastDeliveredOrderId);
            var lastDeliveredOrderPrevStatusId = HttpContext.Session.GetInt32(SessionLastDeliveredOrderPrevStatusId);

            if (lastDeliveredOrderId == null || lastDeliveredOrderPrevStatusId == null)
                return RedirectToAction(nameof(Index)); // No hay última orden o estado previo

            var order = _unitOfWork.Order.Get(o => o.Id == lastDeliveredOrderId, includeProperties: "Status");
            if (order == null)
                return RedirectToAction(nameof(Index));

            // Restaurar el estado previo guardado
            order.StatusId = lastDeliveredOrderPrevStatusId.Value;
            order.LastStatusChange = DateTime.Now;

            _unitOfWork.Order.Update(order);
            _unitOfWork.Save();

            // Limpiar sesión para que no se pueda deshacer más de una vez
            HttpContext.Session.Remove(SessionLastDeliveredOrderId);
            HttpContext.Session.Remove(SessionLastDeliveredOrderPrevStatusId);

            return RedirectToAction(nameof(Index));
        }
    }
}
