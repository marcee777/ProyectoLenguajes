using System.ComponentModel.DataAnnotations;

namespace ProyectoLenguajes.Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Tiempo en minutos para cambiar al siguiente estado
        public int? TimeToNextStatus { get; set; }

        // Id del estado al que debe avanzar automáticamente
        public int? NextStatusId { get; set; }
    }
}
