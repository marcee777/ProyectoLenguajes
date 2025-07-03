using Microsoft.AspNetCore.Mvc;
using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models;
using ProyectoLenguajes.Utilities;
using System.Linq;

namespace ProyectoLenguajes.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StatusController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public StatusController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET
        public IActionResult Index()
        {
            var statuses = _unitOfWork.Status.GetAll()
                .Where(s => s.Name == StaticValues.Status_OnTime || s.Name == StaticValues.Status_OverTime)
                .ToList();

            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(statuses);
        }

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
                TempData["SuccessMessage"] = "Tiempos actualizados correctamente.";
                return RedirectToAction(nameof(Index));
            }

            return View(statuses);
        }
    }
}
