using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProyectoPedidosExpress.Models
{

    /**
    * Modelo que representa el plato
    * @author: Melanie Arce C30634
    * @author: Carolina Rodriguez C36640
    * @author: Marcela Rojas C36975
    * @version: 16/06/25
    */
    public class Plato
    {

        //,d,lc,lesseeeeeeeeeeeeeeeeeee
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [DisplayName("Nombre del Plato")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [DisplayName("Descripcion del Plato")]
        public string Descripcion { get; set; } // texto enriquecido en la vista

        
        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ingrese un precio válido")]
        [DisplayName("Precio del Plato")]
        public decimal Precio { get; set; }

        
        [Display(Name = "Fotografía")]
        public string Fotografia { get; set; } // URL o nombre del archivo guardado

        [Display(Name = "¿Disponible?")]
        public bool Activo { get; set; } // true: disponible, false: inhabilitado temporalmente

    }
}
