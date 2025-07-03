using Microsoft.AspNetCore.Mvc;

namespace ProyectoLenguajes.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
