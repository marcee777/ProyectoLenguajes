using System.ComponentModel.DataAnnotations;

namespace ProyectoLenguajes.Areas.Admin.Models.ViewModels
{
    public class UserVM
    {
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [StringLength(250)]
        public string? Address { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();

    }
}
