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
            var configs = _unitOfWork.StatusTimeConfig.GetAll();
            return View(configs);
        }

        // GET: StatusTimeConfig/Upsert/5
        // o GET: StatusTimeConfig/Upsert (para crear)
        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                // Crear
                return View(new StatusTimeConfig());
            }
            else
            {
                var config = _unitOfWork.StatusTimeConfig.Get(c => c.Id == id);
                if (config == null)
                {
                    return NotFound();
                }
                return View(config);
            }
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

        // DELETE: StatusTimeConfig/Delete/5
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var config = _unitOfWork.StatusTimeConfig.Get(c => c.Id == id);
            if (config == null)
            {
                return Json(new { success = false, message = "Config not found." });
            }

            _unitOfWork.StatusTimeConfig.Remove(config);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleted successfully." });
        }
    }
}
