using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyectoLenguajes.Models;
using ProyectoLenguajes.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoLenguajes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticValues.Role_Admin)]
    public class CustomerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var allUsers = _userManager.Users.ToList();
            var customers = new System.Collections.Generic.List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains(StaticValues.Role_Customer))
                {
                    customers.Add(user);
                }
            }

            return View(customers);
        }

        [HttpPost]
        public async Task<IActionResult> Block(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Json(new { success = false, message = "Customer not found" });

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains(StaticValues.Role_Customer))
                return Json(new { success = false, message = "User is not a customer" });

            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(1));
            return Json(new { success = true, message = "Customer blocked successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> Unblock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Json(new { success = false, message = "Customer not found" });

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains(StaticValues.Role_Customer))
                return Json(new { success = false, message = "User is not a customer" });

            await _userManager.SetLockoutEndDateAsync(user, null);
            return Json(new { success = true, message = "Customer unblocked successfully" });
        }
    }
}
