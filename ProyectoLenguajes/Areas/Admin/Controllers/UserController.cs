using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoLenguajes.Areas.Admin.Views.ViewModel;
using ProyectoLenguajes.Models;
using ProyectoLenguajes.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoLenguajes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticValues.Role_Admin)]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var allUsers = await _userManager.Users.ToListAsync();
            var filteredUsers = new List<ApplicationUser>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any(r => r == StaticValues.Role_Admin || r == StaticValues.Role_Chef))
                    filteredUsers.Add(user);
            }

            return View(filteredUsers);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string? id)
        {
            var roles = _roleManager.Roles
                .Where(r => r.Name == StaticValues.Role_Admin || r.Name == StaticValues.Role_Chef)
                .Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToList();

            var vm = new UserVM { RoleList = roles };

            if (!string.IsNullOrEmpty(id))
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) return NotFound();

                var userRoles = await _userManager.GetRolesAsync(user);
                vm.User = user;
                vm.SelectedRole = userRoles.FirstOrDefault();
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(UserVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ApplicationUser user;
            if (string.IsNullOrEmpty(model.User.Id))
            {
                // Create new user
                user = new ApplicationUser
                {
                    UserName = model.User.Email,
                    Email = model.User.Email,
                    FirstName = model.User.FirstName,
                    LastName = model.User.LastName
                };

                var createResult = await _userManager.CreateAsync(user, "Default123*"); // Consider asking password or email confirmation

                if (!createResult.Succeeded)
                {
                    foreach (var error in createResult.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(model);
                }

                await _userManager.AddToRoleAsync(user, model.SelectedRole);
            }
            else
            {
                // Update existing user
                user = await _userManager.FindByIdAsync(model.User.Id);
                if (user == null) return NotFound();

                user.FirstName = model.User.FirstName;
                user.LastName = model.User.LastName;
                user.Email = model.User.Email;
                user.UserName = model.User.Email;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(model);
                }

                var oldRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, oldRoles);
                await _userManager.AddToRoleAsync(user, model.SelectedRole);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return Json(new { success = false, message = "Error deleting user" });

            return Json(new { success = true, message = "User deleted successfully" });
        }
    }
}
