using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize(Roles = StaticValues.Role_Admin)]
    public class DishController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;



        /**
         * Constructor del controlador DishController
         * Inicializa el repositorio y el entorno de alojamiento web
         * 
         * @param unitOfWork Repositorio para acceso a datos
         * @param webHostEnvironment Proporciona información sobre el entorno web (para rutas de archivos, etc.)
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */
        public DishController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }



        /**
         * Método que muestra la lista de platos disponibles
         * 
         * @return Vista con todos los platos almacenados en la base de datos
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */
        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Dish> dishList = _unitOfWork.Dish.GetAll();
            return View(dishList);
        }



        /**
         * Método que carga el formulario para crear o editar un plato
         * 
         * @param id Identificador del plato (si es nulo o 0, se crea uno nuevo)
         * @return Vista con los datos del plato a crear o editar, o error si no existe el plato a editar
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */
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



        /**
         * Método que guarda un nuevo plato o actualiza uno existente
         * Maneja el procesamiento de la imagen del plato (subida o eliminación) y la persistencia en base de datos
         * 
         * @param dish Objeto Dish con los datos del plato a guardar o actualizar
         * @param file Archivo de imagen del plato (opcional)
         * @return Redirecciona al índice si la operación es exitosa, o regresa a la vista si hay errores de validación
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */

        [HttpPost]
        public IActionResult Upsert(Dish dish, IFormFile? file)
        {
            // Evita que el modelo sea inválido cada que se inserta o edita
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

            return RedirectToAction("Index"); 
        }



        /**
         * Método que elimina un plato existente del sistema
         * También elimina su imagen asociada si no es la predeterminada
         * 
         * @param id Identificador del plato a eliminar
         * @return Objeto JSON con el resultado de la operación (éxito o error con mensaje correspondiente)
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */

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
    }
}
