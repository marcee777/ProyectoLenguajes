using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models.ViewModels;
using ProyectoLenguajes.Utilities;

namespace ProyectoLenguajes.Areas.Kitchen.Controllers
{
    [Area("Kitchen")]
    [Authorize(Roles = StaticValues.Role_Kitchen)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int MaxOrdersToShow = 7;

        // Claves de sesión para guardar último pedido entregado y estado previo
        private const string SessionLastDeliveredOrderId = "_LastDeliveredOrderId";
        private const string SessionLastDeliveredOrderPrevStatusId = "_LastDeliveredOrderPrevStatusId";
        private const string SessionLastDeliveredOrderPrevStatusChange = "_LastDeliveredOrderPrevStatusChange";

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

            var ordersToShow = allActiveOrders.Take(MaxOrdersToShow).ToList();

            var orderVMs = ordersToShow.Select(o => new OrderVM
            {
                Order = o,
                OrderDetails = o.OrderDetails.ToList()
            }).ToList();

            ViewBag.HasMoreOrders = allActiveOrders.Count > MaxOrdersToShow;

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

            // Guardar en sesión el Id del pedido, estado previo y LastStatusChange previo
            HttpContext.Session.SetInt32(SessionLastDeliveredOrderId, order.Id);
            HttpContext.Session.SetInt32(SessionLastDeliveredOrderPrevStatusId, order.StatusId);
            HttpContext.Session.SetString(SessionLastDeliveredOrderPrevStatusChange, order.LastStatusChange.ToString("o")); // ISO 8601

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
            var lastDeliveredOrderPrevStatusChangeStr = HttpContext.Session.GetString(SessionLastDeliveredOrderPrevStatusChange);

            if (lastDeliveredOrderId == null || lastDeliveredOrderPrevStatusId == null || lastDeliveredOrderPrevStatusChangeStr == null)
                return RedirectToAction(nameof(Index));

            if (!DateTime.TryParse(lastDeliveredOrderPrevStatusChangeStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out var lastDeliveredOrderPrevStatusChange))
                return RedirectToAction(nameof(Index)); // fallback en caso de error al parsear

            var order = _unitOfWork.Order.Get(o => o.Id == lastDeliveredOrderId, includeProperties: "Status");
            if (order == null)
                return RedirectToAction(nameof(Index));

            order.StatusId = lastDeliveredOrderPrevStatusId.Value;
            order.LastStatusChange = lastDeliveredOrderPrevStatusChange;

            _unitOfWork.Order.Update(order);
            _unitOfWork.Save();

            // Limpiar sesión para evitar deshacer repetido
            HttpContext.Session.Remove(SessionLastDeliveredOrderId);
            HttpContext.Session.Remove(SessionLastDeliveredOrderPrevStatusId);
            HttpContext.Session.Remove(SessionLastDeliveredOrderPrevStatusChange);

            return RedirectToAction(nameof(Index));
        }
    }
}
