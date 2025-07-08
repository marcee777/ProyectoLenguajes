using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Policy;

namespace ProyectoLenguajes.Models
{

    /**
      * Clase Dish
      * 
      * Representa el modelo de un plato en el sistema con propiedades para
      * Id, nombre, descripción, precio, imagen y estado de disponibilidad.
      * Incluye validaciones y anotaciones para la presentación y requerimientos de datos.
      * 
      * @author Melanie Arce C30634
      * @author Carolina Rodriguez C36640
      * @author Marcela Rojas C36975
      * @version 07/07/25
      */

    public class Dish
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The name is required")]
        [DisplayName("Dish Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The description is required")]
        [DisplayName("Dish Description")]
        public string Description { get; set; } // texto enriquecido en la vista

        [Required(ErrorMessage = "The price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Enter a valid price")]
        [DisplayName("Dish Price")]
        public decimal Price { get; set; }

        [Display(Name = "Photo")]
        public string URLImage { get; set; } // URL o nombre del archivo guardado

        [Display(Name = "Available?")]
        public bool IsActive { get; set; } // true: disponible, false: inhabilitado temporalmente
    }
}
