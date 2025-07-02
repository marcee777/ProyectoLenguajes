using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

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
        public List<OrderDetail> OrderDetails { get; set; }


   
    }
}
