using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoLenguajes.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
