using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoLenguajes.Data;
using ProyectoLenguajes.Areas.Admin.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using ProyectoLenguajes.Utilities;
using Microsoft.AspNetCore.Identity;

namespace ProyectoLenguajes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Utilities.StaticValues.Role_Admin)]

    /**
     * Controlador que gestiona las operaciones administrativas relacionadas con las órdenes de los clientes.
     * Permite visualizar órdenes con filtros, y actualizar el estado de las mismas.
     * Utiliza el contexto de base de datos para obtener y modificar la información.
     * 
     * Este controlador está restringido para usuarios con el rol de administrador.
     * 
     * @author: Melanie Arce C30634
     * @author: Carolina Rodríguez C36640
     * @author: Marcela Rojas C36975
     * @version: 07/07/25
     */

    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        /**
         * Constructor del controlador OrderController
         * Inicializa el contexto de base de datos para acceder a las órdenes y sus relaciones,
         * así como el servicio UserManager para gestionar usuarios y sus roles.
         * 
         * @param dbContext Contexto de la base de datos para acceder a las entidades relacionadas con órdenes
         * @param userManager Servicio para manipular entidades de usuario y sus roles en ASP.NET Identity
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */
        public OrderController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        /**
         * Método que muestra la lista de órdenes con opción de filtrar por cliente, estado y fechas
         * 
         * @param clientId Identificador del cliente (opcional) para filtrar las órdenes por usuario
         * @param statusId Identificador del estado de la orden (opcional) para filtrar por estado específico
         * @param startDate Fecha de inicio (opcional) para filtrar órdenes desde una fecha específica
         * @param endDate Fecha de fin (opcional) para filtrar órdenes hasta una fecha específica
         * 
         * @return Vista con un ViewModel que contiene la lista de órdenes filtradas y los selectores para clientes y estados disponibles en el sistema
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */
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
            var allUsers = await _userManager.Users.ToListAsync();
            var customerUsers = new List<SelectListItem>();
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains(StaticValues.Role_Customer))
                {
                    customerUsers.Add(new SelectListItem
                    {
                        Value = user.Id,
                        Text = user.UserName
                    });
                }
            }

            var model = new OrderFilterVM
            {
                ClientId = clientId,
                StatusId = statusId,
                StartDate = startDate,
                EndDate = endDate,
                Clients = customerUsers,
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

        /**
         * Método que actualiza el estado de una orden específica
         * 
         * @param orderId Identificador de la orden a actualizar
         * @param newStatusId Nuevo estado que se asignará a la orden
         * 
         * @return Redirecciona al índice si la operación es exitosa; retorna NotFound si la orden no existe en la base de datos
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */

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

            TempData["success"] = "Order status updated successfully";

            return RedirectToAction(nameof(Index));
        }
    }

}
