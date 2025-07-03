using System.ComponentModel.DataAnnotations;

namespace ProyectoLenguajes.Models
{
    public class StatusTimeConfig
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string StatusName { get; set; } // Ejemplo: "On Time"

        [Required]
        public int MinutesToNextState { get; set; } // Tiempo en minutos para cambiar

        [Required]
        public string NextStatusName { get; set; } // Ejemplo: "Over Time"
    }
}
