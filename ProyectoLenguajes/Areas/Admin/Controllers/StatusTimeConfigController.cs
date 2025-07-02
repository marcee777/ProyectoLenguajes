using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class StatusTimeConfigController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StatusTimeConfigController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: StatusTimeConfig
        public IActionResult Index()
        {
            // Para evitar múltiples configuraciones, redirige directo a editar la única configuración
            var config = _unitOfWork.StatusTimeConfig.Get(c => true);
            if (config == null)
            {
                return RedirectToAction(nameof(Upsert));
            }
            else
            {
                return RedirectToAction(nameof(Upsert), new { id = config.Id });
            }
        }

        // GET: StatusTimeConfig/Upsert
        public IActionResult Upsert(int? id)
        {
            StatusTimeConfig config;

            if (id == null || id == 0)
            {
                // Intenta buscar el único registro
                config = _unitOfWork.StatusTimeConfig.Get(c => true);
                if (config == null)
                {
                    config = new StatusTimeConfig(); // para crear nuevo
                }
            }
            else
            {
                config = _unitOfWork.StatusTimeConfig.Get(c => c.Id == id);
                if (config == null)
                {
                    return NotFound();
                }
            }

            return View(config);
        }

        // POST: StatusTimeConfig/Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(StatusTimeConfig config)
        {
            if (ModelState.IsValid)
            {
                if (config.Id == 0)
                {
                    // Antes de agregar, elimina todas las configuraciones viejas por seguridad
                    var existingConfigs = _unitOfWork.StatusTimeConfig.GetAll();
                    foreach (var c in existingConfigs)
                    {
                        _unitOfWork.StatusTimeConfig.Remove(c);
                    }
                    _unitOfWork.StatusTimeConfig.Add(config);
                }
                else
                {
                    _unitOfWork.StatusTimeConfig.Update(config);
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(config);
        }
    }
}
