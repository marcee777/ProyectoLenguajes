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

    /**
     * Controlador que gestiona la administración de usuarios en el área de administrador.
     * Permite listar, buscar, actualizar, eliminar, bloquear y desbloquear usuarios.
     * Utiliza Identity para gestionar la información de autenticación y roles.
     * 
     * Este controlador está restringido a usuarios con el rol de administrador.
     * 
     * Funciona sobre el modelo personalizado ApplicationUser y el ViewModel UserVM.
     * 
     * @author: Melanie Arce C30634
     * @author: Carolina Rodríguez C36640
     * @author: Marcela Rojas C36975
     * @version: 07/07/25
     */
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        /**
         * Constructor del controlador UserController
         * Inicializa los servicios de gestión de usuarios y roles provistos por ASP.NET Identity
         * 
         * @param userManager Servicio para manipular entidades de usuario
         * @param roleManager Servicio para manipular roles de usuario en el sistema de identidad de ASP.NET Core
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */
        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/User
        /**
         * Método que muestra la lista de usuarios del sistema con opción de búsqueda por nombre, apellido o correo electrónico.
         * También obtiene los roles e información extendida de cada usuario.
         * 
         * @param search Cadena de búsqueda opcional para filtrar usuarios por nombre, apellido o correo
         * @return Vista con la lista de usuarios transformada en el ViewModel UserVM, incluyendo estado de bloqueo y roles asignados.
         * 
         * Nota: Se excluyen usuarios que no implementen ApplicationUser como tipo base.
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */
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

        /**
         * Método que carga el formulario para editar la información de un usuario específico.
         * 
         * @param id Identificador del usuario a editar
         * @return Vista con los datos cargados en el ViewModel UserVM o errores si no se encuentra el usuario o no es del tipo esperado (ApplicationUser)
         * 
         * Valida existencia del usuario y su tipo antes de mostrar la vista de edición.
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */
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


        /**
         * Método que procesa la edición de un usuario.
         * Valida que el modelo sea correcto, busca el usuario, actualiza sus datos y guarda los cambios.
         * 
         * @param model Objeto UserVM con los nuevos datos del usuario a actualizar
         * @return Redirecciona al índice si se guarda correctamente; si hay errores, los muestra en la vista de edición
         * 
         * Usa el UserManager para actualizar el objeto ApplicationUser asociado al usuario.
         * Protegido contra CSRF con el token de validación antifalsificación.
         * 
         * @see UserVM
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */

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
            {
                TempData["success"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // POST: Admin/User/Delete/5


        /**
         * Método que elimina un usuario del sistema de forma permanente.
         * 
         * @param id Identificador del usuario a eliminar
         * @return Redirecciona al índice con un mensaje en TempData que indica si la operación fue exitosa o no
         * 
         * Valida existencia y tipo del usuario antes de proceder a la eliminación.
         * Protegido contra CSRF con token de validación antifalsificación.
         * 
         * @see ApplicationUser
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */

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

        /**
         * Método que bloquea a un usuario con rol "Customer" por 30 minutos.
         * 
         * @param id Identificador del usuario a bloquear
         * @return Redirecciona al índice con mensaje de éxito o error si el usuario no cumple los criterios
         * 
         * Aplica un bloqueo temporal mediante LockoutEnd en UTC.
         * Solo usuarios con rol "Customer" pueden ser bloqueados.
         * Protegido contra CSRF.
         * 
         * @see ApplicationUser
         * 
         * @see UserManager.SetLockoutEndDateAsync
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */

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

        /**
         * Método que desbloquea a un usuario con rol "Customer", eliminando la fecha de bloqueo.
         * 
         * @param id Identificador del usuario a desbloquear
         * @return Redirecciona al índice con mensaje de éxito o error si no se cumple alguna condición
         * 
         * Solo usuarios con el rol "Customer" pueden ser desbloqueados.
         * Protegido contra CSRF.
         * 
         * @see ApplicationUser
         * 
         * @see UserManager.SetLockoutEndDateAsync
         * 
         * @author: Melanie Arce C30634
         * @author: Carolina Rodríguez C36640
         * @author: Marcela Rojas C36975
         * @version: 07/07/25
         */

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
