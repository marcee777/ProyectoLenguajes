using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ProyectoLenguajes.Models;
using ProyectoLenguajes.Utilities;

namespace ProyectoLenguajes.Areas.Admin.Api
{
    [Area("Admin")]
    [Route("Admin/Api/[controller]")]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerApiController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: Admin/api/CustomerApi
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var allUsers = _userManager.Users.ToList();
            var customers = new List<object>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains(StaticValues.Role_Customer))
                {
                    customers.Add(new
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Address = user.Address,
                        Email = user.Email,
                        IsBlocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow
                    });
                }
            }

            return Ok(customers);
        }

        // POST: Admin/api/CustomerApi/block/{id}
        [HttpPost("block/{id}")]
        public async Task<IActionResult> Block(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return NotFound(new { Success = false, Message = "Customer not found" });

                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains(StaticValues.Role_Customer))
                    return BadRequest(new { Success = false, Message = "User is not a customer" });

                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(1));
                return Ok(new { Success = true, Message = "Customer blocked successfully" });
            }
            catch
            {
                return StatusCode(500, new { Success = false, Message = "Unexpected error while blocking customer." });
            }
        }

        // POST: Admin/api/CustomerApi/unblock/{id}
        [HttpPost("unblock/{id}")]
        public async Task<IActionResult> Unblock(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return NotFound(new { Success = false, Message = "Customer not found" });

                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains(StaticValues.Role_Customer))
                    return BadRequest(new { Success = false, Message = "User is not a customer" });

                await _userManager.SetLockoutEndDateAsync(user, null);
                return Ok(new { Success = true, Message = "Customer unblocked successfully" });
            }
            catch
            {
                return StatusCode(500, new { Success = false, Message = "Unexpected error while unblocking customer." });
            }
        }
    }
}
