using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models.ViewModels;
using ProyectoLenguajes.Utilities;

namespace ProyectoLenguajes.Areas.Kitchen.Controllers
{
    [Area("Kitchen")]
    [Authorize(Roles = StaticValues.Role_Kitchen)]

    /*
     * Controlador encargado de gestionar la cola de pedidos activos en el área de cocina.
     * Permite listar los pedidos que están en proceso, marcarlos como entregados y revertir la última acción.
     *
     * @author Melanie Arce C30634
     * @author Carolina Rodríguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

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
        /*
         * Muestra en la vista los primeros 8 pedidos activos (con estados OnTime, OverTime o Delayed)
         * que están pendientes de ser preparados o entregados por la cocina.
         * También verifica si existen más pedidos en cola y carga la última entrega marcada desde sesión.
         *
         * @return Vista con los pedidos activos que debe visualizar el personal de cocina.
         *
         * @author Melanie Arce C30634
         * @author Carolina Rodríguez C36640
         * @author Marcela Rojas C36975
         * @version 07/07/25
         */

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


        /*
         * Devuelve un listado en formato JSON de los pedidos activos, incluyendo detalles del cliente
         * y los platos solicitados. Este endpoint es útil para llamadas AJAX desde la interfaz de cocina.
         *
         * @return JSON con la lista de pedidos activos y un indicador si hay más de 8.
         *
         * @author Melanie Arce C30634
         * @author Carolina Rodríguez C36640
         * @author Marcela Rojas C36975
         * @version 07/07/25
         */

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
        /*
         * Marca un pedido específico como entregado, cambiando su estado a "Delivered".
         * Guarda en la sesión el estado anterior del pedido para permitir deshacer esta acción si es necesario.
         *
         * @param id Identificador del pedido que se desea marcar como entregado.
         * @return JSON con mensaje de éxito o error según la operación realizada.
         *
         * @author Melanie Arce C30634
         * @author Carolina Rodríguez C36640
         * @author Marcela Rojas C36975
         * @version 07/07/25
         */

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

        /*
         * Deshace la última acción de entrega registrada, restaurando el estado y fecha anterior del pedido
         * a partir de los datos almacenados en la sesión.
         *
         * @return JSON con un mensaje indicando si la reversión fue exitosa o si no hay datos para restaurar.
         *
         * @author Melanie Arce C30634
         * @author Carolina Rodríguez C36640
         * @author Marcela Rojas C36975
         * @version 07/07/25
         */

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
