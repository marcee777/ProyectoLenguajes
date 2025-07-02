using System.ComponentModel.DataAnnotations;

namespace ProyectoLenguajes.Models
{
    public class ApplicationUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [StringLength(11)]
        [MinLength(11)]
        [MaxLength(11)]
        [RegularExpression(@"^\d{1}-\d{4}-\d{4}$", ErrorMessage = "The format must be X-XXXX-XXXX, where X is a digit.")]
        public string Identification { get; set; } //  "Cédula"
    }
}
