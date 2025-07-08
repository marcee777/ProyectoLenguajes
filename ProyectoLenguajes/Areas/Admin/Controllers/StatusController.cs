using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models;
using ProyectoLenguajes.Utilities;
using System.Linq;

namespace ProyectoLenguajes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticValues.Role_Admin)]

    /**
     * Controlador que permite al administrador visualizar y actualizar los tiempos de transición de estados
     * utilizados para controlar el flujo de las órdenes (por ejemplo: "A tiempo", "Fuera de tiempo").
     * 
     * Este controlador está restringido a usuarios con el rol de administrador.
     * Interactúa con el repositorio de estados para obtener y actualizar la información en la base de datos.
     * 
     * @author: Melanie Arce C30634
     * @author: Carolina Rodríguez C36640
     * @author: Marcela Rojas C36975
     * @version: 07/07/25
     */
    public class StatusController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;


        /**
         * Constructor del controlador StatusController
         * Inicializa la unidad de trabajo para acceder a los estados almacenados en la base de datos
         * 
         * @param unitOfWork Objeto que proporciona acceso a los repositorios de la base de datos (Status, etc.) mediante inyección de dependencias
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */
        public StatusController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        /**
         * Método que carga la vista con los estados "A tiempo" y "Fuera de tiempo"
         * para que el administrador pueda visualizar sus configuraciones actuales.
         * También gestiona el mensaje de éxito si fue redirigido tras una actualización.
         * 
         * @return Vista que contiene la lista de estados filtrados según el nombre del estado (solo los editables por tiempo)
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */
        // GET
        public IActionResult Index()
        {
            var statuses = _unitOfWork.Status.GetAll()
                .Where(s => s.Name == StaticValues.Status_OnTime || s.Name == StaticValues.Status_OverTime)
                .ToList();

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(statuses);
        }




        /**
         * Método que carga la vista con los estados "A tiempo" y "Fuera de tiempo"
         * para que el administrador pueda visualizar sus configuraciones actuales.
         * También gestiona el mensaje de éxito si fue redirigido tras una actualización.
         * 
         * @return Vista que contiene la lista de estados filtrados según el nombre del estado (solo los editables por tiempo)
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */
        // POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(List<Status> statuses)
        {
            if (statuses != null && statuses.Count > 0)
            {
                foreach (var status in statuses)
                {
                    var statusDb = _unitOfWork.Status.Get(s => s.Id == status.Id);
                    if (statusDb != null)
                    {
                        statusDb.TimeToNextStatus = status.TimeToNextStatus;
                        _unitOfWork.Status.Update(statusDb);
                    }
                }

                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Configuration updated succesfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(statuses);
        }
    }
}
