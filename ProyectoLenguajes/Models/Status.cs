using System.ComponentModel.DataAnnotations;

namespace ProyectoLenguajes.Models
{
    /**
     * Clase Status
     * 
     * Representa el estado de una orden dentro del sistema, incluyendo el nombre del estado,
     * el tiempo estimado en minutos para avanzar al siguiente estado y el ID del próximo estado.
     * Esta clase ayuda a gestionar el flujo y seguimiento de las órdenes.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodriguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

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
