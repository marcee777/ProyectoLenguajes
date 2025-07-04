using Microsoft.AspNetCore.Mvc;
using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models;
using System.Linq;

namespace ProyectoLenguajes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Api/[controller]")]
    [ApiController]
    public class DishApiController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DishApiController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/DishApi
        [HttpGet]
        public IActionResult GetAll()
        {
            var dishList = _unitOfWork.Dish.GetAll()
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.Description,
                    d.Price,
                    d.URLImage,
                    d.IsActive
                }).ToList();

            return Ok(dishList);
        }

        // GET: api/DishApi/active
        [HttpGet("active")]
        public IActionResult GetActive()
        {
            var activeDishes = _unitOfWork.Dish.GetAll()
                .Where(d => d.IsActive)
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.Description,
                    d.Price,
                    d.URLImage,
                    d.IsActive
                }).ToList();

            return Ok(activeDishes);
        }

        // DELETE: api/DishApi/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var dishToDelete = _unitOfWork.Dish.Get(d => d.Id == id);

            if (dishToDelete == null)
            {
                return NotFound(new { Success = false, Message = "Dish not found" });
            }

            _unitOfWork.Dish.Remove(dishToDelete);
            _unitOfWork.Save();

            return Ok(new { Success = true, Message = "Dish deleted successfully" });
        }
    }
}
