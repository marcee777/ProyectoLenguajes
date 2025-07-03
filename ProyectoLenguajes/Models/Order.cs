using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoLenguajes.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ClientId { get; set; } // FK a ApplicationUser.Id (que es string)

        [ForeignKey("ClientId")]
        public ApplicationUser Client { get; set; } // navigation property

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public int StatusId { get; set; }  // FK hacia Status

        [ForeignKey("StatusId")]
        public Status Status { get; set; } // Navigation property

        public List<OrderDetail> OrderDetails { get; set; }

        public Order()
        {
            CreatedAt = DateTime.Now;
        }

    }
}