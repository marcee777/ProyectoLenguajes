using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ProyectoLenguajes.Models;
using Microsoft.EntityFrameworkCore;
using ProyectoLenguajes.Areas.Admin.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using ProyectoLenguajes.Utilities;

namespace ProyectoLenguajes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticValues.Role_Admin)]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/User
        public async Task<IActionResult> Index(string search)
        {
            var users = await _userManager.Users.ToListAsync();

            var userList = users
                .Select(async userIdentity =>
                {
                    var user = userIdentity as ApplicationUser;
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        return new
                        {
                            User = user,
                            Roles = roles
                        };
                    }
                    return null;
                })
                .Select(t => t.Result)
                .Where(u => u != null)
                .ToList();

            // Filtrar si hay búsqueda
            if (!string.IsNullOrEmpty(search))
            {
                userList = userList
                    .Where(u => u.User.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase)
                             || u.User.LastName.Contains(search, StringComparison.OrdinalIgnoreCase)
                             || u.User.Email.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Preparar modelo
            var model = userList.Select(u => new UserVM
            {
                Id = u.User.Id,
                FirstName = u.User.FirstName,
                LastName = u.User.LastName,
                Email = u.User.Email,
                Address = u.User.Address,
                Roles = u.Roles,
                IsBlocked = u.User.LockoutEnd.HasValue && u.User.LockoutEnd > DateTimeOffset.UtcNow
            }).ToList();

            return View(model);
        }

        // GET: Admin/User/Update/5
        public async Task<IActionResult> Update(string id)
        {
            if (id == null) return NotFound();

            var userIdentity = await _userManager.FindByIdAsync(id);
            if (userIdentity == null) return NotFound();

            var user = userIdentity as ApplicationUser;
            if (user == null) return BadRequest("User is not of type ApplicationUser.");

            var roles = await _userManager.GetRolesAsync(userIdentity);

            var model = new UserVM
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Address = user.Address,
                Roles = roles,
                IsBlocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow
            };

            return View(model);
        }

        // POST: Admin/User/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userIdentity = await _userManager.FindByIdAsync(model.Id);
            if (userIdentity == null)
                return NotFound();

            var user = userIdentity as ApplicationUser;
            if (user == null)
                return BadRequest("User is not of type ApplicationUser.");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.Address = model.Address;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return RedirectToAction(nameof(Index));

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // POST: Admin/User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var userIdentity = await _userManager.FindByIdAsync(id);
            if (userIdentity == null)
                return RedirectToAction(nameof(Index));

            var user = userIdentity as ApplicationUser;
            if (user == null)
                return RedirectToAction(nameof(Index));

            var result = await _userManager.DeleteAsync(user);

            TempData[result.Succeeded ? "success" : "error"] =
                result.Succeeded ? "User deleted successfully." : "Failed to delete user.";

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/User/Block/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block(string id)
        {
            var userIdentity = await _userManager.FindByIdAsync(id);
            if (userIdentity == null) return RedirectToAction(nameof(Index));

            var user = userIdentity as ApplicationUser;
            if (user == null) return RedirectToAction(nameof(Index));

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Customer"))
            {
                TempData["error"] = "Only customers can be blocked.";
                return RedirectToAction(nameof(Index));
            }

            var lockoutEnd = DateTimeOffset.UtcNow.AddMinutes(30);
            await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);

            TempData["success"] = $"Customer blocked until {lockoutEnd:u} UTC.";
            return RedirectToAction(nameof(Index));
        }


        // POST: Admin/User/Unblock/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unblock(string id)
        {
            var userIdentity = await _userManager.FindByIdAsync(id);
            if (userIdentity == null) return RedirectToAction(nameof(Index));

            var user = userIdentity as ApplicationUser;
            if (user == null) return RedirectToAction(nameof(Index));

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Customer"))
            {
                TempData["error"] = "Only customers can be unblocked.";
                return RedirectToAction(nameof(Index));
            }

            await _userManager.SetLockoutEndDateAsync(user, null);
            TempData["success"] = "Customer unblocked.";
            return RedirectToAction(nameof(Index));
        }
    }
}
