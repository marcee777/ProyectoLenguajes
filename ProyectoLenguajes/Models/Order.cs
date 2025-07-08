using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoLenguajes.Models
{
    /**
     * Clase Order
     * 
     * Modelo que representa una orden realizada por un cliente.
     * Contiene referencias al cliente, estado de la orden, detalles de los platillos,
     * y fechas importantes como la creación y el último cambio de estado.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodriguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ClientId { get; set; } // FK a ApplicationUser.Id (que es string)

        [ForeignKey("ClientId")]
        public ApplicationUser Client { get; set; } // Navigation property

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public int StatusId { get; set; }  // FK hacia Status

        [ForeignKey("StatusId")]
        public Status Status { get; set; } // Navigation property

        public DateTime LastStatusChange { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }

        public Order()
        {
            CreatedAt = DateTime.Now;
            LastStatusChange = CreatedAt;
        }

    }
}