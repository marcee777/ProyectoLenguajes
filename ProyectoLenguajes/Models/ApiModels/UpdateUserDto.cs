using System.ComponentModel.DataAnnotations;

namespace ProyectoLenguajes.Models.ApiModels
{
    public class UpdateUserDto
    {
       
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = null!;

        [StringLength(250)]
        public string? Address { get; set; }

        [StringLength(100, MinimumLength = 6)]
        public string? NewPassword { get; set; }
    }
}
