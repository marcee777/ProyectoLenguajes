using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ProyectoLenguajes.Models;
using Microsoft.EntityFrameworkCore;
using ProyectoLenguajes.Areas.Admin.Models.ViewModels;

namespace ProyectoLenguajes.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            var users = _userManager.Users;

            if (!string.IsNullOrEmpty(search))
            {
                // Aquí hay un problema porque IdentityUser no tiene FirstName ni LastName,
                // así que para filtrar por esos campos tendrías que castear o filtrar en memoria.
                // Ejemplo simple (¡ojo, podría afectar performance!):
                var userList = await users.ToListAsync();
                var filtered = userList
                    .Cast<ApplicationUser>()
                    .Where(u => u.FirstName.Contains(search) ||
                                u.LastName.Contains(search) ||
                                u.Email.Contains(search))
                    .ToList();
                return View(filtered);
            }

            var userListAll = await users.ToListAsync();
            return View(userListAll.Cast<ApplicationUser>().ToList());
        }

        // GET: Admin/User/Update/5
        public async Task<IActionResult> Update(string id)
        {
            if (id == null)
                return NotFound();

            var userIdentity = await _userManager.FindByIdAsync(id);
            if (userIdentity == null)
                return NotFound();

            var user = userIdentity as ApplicationUser;
            if (user == null)
                return BadRequest("Usuario no es del tipo ApplicationUser.");

            var roles = await _userManager.GetRolesAsync(userIdentity);

            var model = new UserVM
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Address = user.Address,
                Roles = roles
            };

            return View(model);
        }

        // POST: Admin/User/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdentity = await _userManager.FindByIdAsync(model.Id);

            if (userIdentity == null)
            {
                return NotFound();
            }

            var user = userIdentity as ApplicationUser;

            if (user == null)
            {
                return BadRequest("Usuario no es del tipo ApplicationUser.");
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.Address = model.Address;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // POST: Admin/User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }  

            var userIdentity = await _userManager.FindByIdAsync(id);

            if (userIdentity == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var user = userIdentity as ApplicationUser;

            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["success"] = "Usuario eliminado correctamente.";
            }
            else
            {
                TempData["error"] = "No se pudo eliminar el usuario.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
