using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Areas.Admin.Views.ViewModel
{
    public class UserVM
    {
        [ValidateNever]
        public ApplicationUser User { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string SelectedRole { get; set; }

        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}
