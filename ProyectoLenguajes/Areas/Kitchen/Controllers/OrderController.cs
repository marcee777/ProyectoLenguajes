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
        private const int MaxOrdersToShow = 8;

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
                .Where(o => o.Status.Name == StaticValues.Status_OnTime ||
                    o.Status.Name == StaticValues.Status_OverTime ||
                    o.Status.Name == StaticValues.Status_Delayed)
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

        [HttpGet]
        public IActionResult GetActiveOrders()
        {
            var allActiveOrders = _unitOfWork.Order
                .GetAll(includeProperties: "Client,OrderDetails.Dish,Status")
                .Where(o => o.Status.Name == StaticValues.Status_OnTime ||
                    o.Status.Name == StaticValues.Status_OverTime ||
                    o.Status.Name == StaticValues.Status_Delayed)
                .OrderBy(o => o.CreatedAt)
                .ToList();

            var ordersToShow = allActiveOrders.Take(MaxOrdersToShow).Select(o => new
            {
                Id = o.Id,
                CreatedAt = o.CreatedAt.ToString("g"),
                ClientName = $"{o.Client.FirstName} {o.Client.LastName}",
                StatusName = o.Status.Name,
                Details = o.OrderDetails.Select(d => new
                {
                    DishName = d.Dish.Name,
                    Amount = d.Amount
                })
            }).ToList();

            return Json(new
            {
                Orders = ordersToShow,
                HasMoreOrders = allActiveOrders.Count > MaxOrdersToShow
            });
        }

        // POST: Marcar como entregado
        [HttpPost]
        [IgnoreAntiforgeryToken] // Solo si se decide no enviar token, o manejar token manual
        public IActionResult MarkAsDelivered([FromBody] int id)
        {
            var order = _unitOfWork.Order.Get(o => o.Id == id, includeProperties: "Status");
            if (order == null)
                return NotFound(new { message = "Order not found." });

            var deliveredStatus = _unitOfWork.Status.Get(s => s.Name == StaticValues.Status_Delivered);
            if (deliveredStatus == null)
                return BadRequest(new { message = "Delivered status not found." });

            HttpContext.Session.SetInt32(SessionLastDeliveredOrderId, order.Id);
            HttpContext.Session.SetInt32(SessionLastDeliveredOrderPrevStatusId, order.StatusId);
            HttpContext.Session.SetString(SessionLastDeliveredOrderPrevStatusChange, order.LastStatusChange.ToString("o"));

            order.StatusId = deliveredStatus.Id;
            order.LastStatusChange = DateTime.Now;

            _unitOfWork.Order.Update(order);
            _unitOfWork.Save();

            return Ok(new { message = "Order marked as delivered." });
        }

        // POST: Deshacer último cambio a entregado
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult UndoLastDelivered()
        {
            var lastDeliveredOrderId = HttpContext.Session.GetInt32(SessionLastDeliveredOrderId);
            var lastDeliveredOrderPrevStatusId = HttpContext.Session.GetInt32(SessionLastDeliveredOrderPrevStatusId);
            var lastDeliveredOrderPrevStatusChangeStr = HttpContext.Session.GetString(SessionLastDeliveredOrderPrevStatusChange);

            if (lastDeliveredOrderId == null || lastDeliveredOrderPrevStatusId == null || lastDeliveredOrderPrevStatusChangeStr == null)
                return BadRequest(new { message = "No previous delivered order to undo." });

            if (!DateTime.TryParse(lastDeliveredOrderPrevStatusChangeStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out var lastDeliveredOrderPrevStatusChange))
                return BadRequest(new { message = "Could not parse previous LastStatusChange." });

            var order = _unitOfWork.Order.Get(o => o.Id == lastDeliveredOrderId, includeProperties: "Status");
            if (order == null)
                return NotFound(new { message = "Order not found." });

            order.StatusId = lastDeliveredOrderPrevStatusId.Value;
            order.LastStatusChange = lastDeliveredOrderPrevStatusChange;

            _unitOfWork.Order.Update(order);
            _unitOfWork.Save();

            // Limpiar sesión
            HttpContext.Session.Remove(SessionLastDeliveredOrderId);
            HttpContext.Session.Remove(SessionLastDeliveredOrderPrevStatusId);
            HttpContext.Session.Remove(SessionLastDeliveredOrderPrevStatusChange);

            return Ok(new { message = "Undo successful." });
        }
    }
}
