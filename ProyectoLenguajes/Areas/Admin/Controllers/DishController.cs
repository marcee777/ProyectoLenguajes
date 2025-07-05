using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoLenguajes.Data.Repository;
using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models;
using ProyectoLenguajes.Utilities;

namespace ProyectoLenguajes.Areas.Admin.Controllers
{
    /**
    * Controlador del plato
    * @author: Melanie Arce C30634
    * @author: Carolina Rodriguez C36640
    * @author: Marcela Rojas C36975
    * @version: 16/06/25
    */

    [Area("Admin")]
    //[Authorize(Roles = StaticValues.Role_Admin)]

    public class DishController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;


        public DishController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Dish> dishList = _unitOfWork.Dish.GetAll();
            return View(dishList);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            Dish dish = new Dish();

            if (id == null || id == 0)
            {
                // Crear nuevo plato
                return View(dish);
            }

            // Editar plato existente
            dish = _unitOfWork.Dish.Get(d => d.Id == id);

            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);

        }

        [HttpPost]
        public IActionResult Upsert(Dish dish, IFormFile? file)
        {
            //Evita que el modelo sea inválido cada que se inserta o edita
            ModelState.Remove("URLImage");

            if (ModelState.IsValid)
            {
                

                if (file != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(file.FileName);
                    var uploadsFolder = Path.Combine(wwwRootPath, @"images\dishes");

                    // Eliminar imagen anterior si no es la predeterminada
                    if (dish.URLImage != null)
                    {
                        var oldImageURL = Path.Combine(wwwRootPath, dish.URLImage);


                        if (oldImageURL != Path.Combine(uploadsFolder, Utilities.StaticValues.Image_DefaultName))
                        {
                            if (System.IO.File.Exists(oldImageURL))
                            {
                                System.IO.File.Delete(oldImageURL);
                            }
                        }
                           
                    }

                    // Guardar nueva imagen
                    using (var fileStream = new FileStream(Path.Combine(uploadsFolder, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    dish.URLImage = @"images\dishes\" + fileName + extension;
                }
                else if (dish.Id == 0)
                {
                    // Imagen por defecto solo al crear
                    dish.URLImage = @"images\dishes\default.jpg";
                }

                // Agregar o actualizar
                if (dish.Id == 0)
                {
                    _unitOfWork.Dish.Add(dish);
                }
                else
                {
                    _unitOfWork.Dish.Update(dish);
                }

                _unitOfWork.Save();
                TempData["success"] = "Dish saved successfully";

            }
            //luego cambiar 
            return RedirectToAction("Index"); 
        }

        #region API

        [HttpGet]
        public IActionResult GetAll()
        {
            var dishList = _unitOfWork.Dish.GetAll();
            return Json(new { data = dishList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var dishToDelete = _unitOfWork.Dish.Get(x => x.Id == id);

            if (dishToDelete == null) 
            {
                return Json(new { success = false, message = "Error deleting dish" });
            }

            // Eliminar imagen del servidor si no es la predeterminada
            if (dishToDelete.URLImage != null && !dishToDelete.URLImage.Contains(Utilities.StaticValues.Image_DefaultName))
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string imagePath = Path.Combine(wwwRootPath, dishToDelete.URLImage);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _unitOfWork.Dish.Remove(dishToDelete);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Dish deleted successfully" });
        }

        #endregion

    }
}
