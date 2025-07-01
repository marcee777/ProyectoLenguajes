using System.ComponentModel.DataAnnotations;

namespace ProyectoLenguajes.Models
{
    public class Usuario
    {

        // Propiedades con setters y getters
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        [StringLength(11)]
        [MinLength(11)]
        [MaxLength(11)]
        [RegularExpression(@"^\d{1}-\d{4}-\d{4}$", ErrorMessage = "El formato debe ser X-XXXX-XXXX, donde X es un dígito.")]
        public string Cedula { get; set; }

        //public IList<string>? Roles { get; set; }


    }
}
