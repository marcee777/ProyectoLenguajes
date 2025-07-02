using System.ComponentModel.DataAnnotations;

namespace ProyectoLenguajes.Models
{
    public class StatusTimeConfig
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MinutesPerStatusChange { get; set; } // Tiempo en minutos entre cada cambio de estado
    }
}
