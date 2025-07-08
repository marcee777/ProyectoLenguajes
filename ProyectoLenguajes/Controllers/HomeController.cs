using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyectoLenguajes.Models;


namespace ProyectoLenguajes.Controllers;
/*
 * Controlador principal de la aplicaci�n, encargado de manejar las vistas generales
 * como la p�gina de inicio (Index) y la vista de errores. Tambi�n registra actividad
 * mediante el sistema de logging inyectado.
 *
 * Esta clase es utilizada como punto de entrada com�n para usuarios no autenticados
 * o navegaci�n b�sica dentro del sistema.
 *
 * @author Melanie Arce C30634
 * @author Carolina Rodr�guez C36640
 * @author Marcela Rojas C36975
 * @version 07/07/25
 */

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;


    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
