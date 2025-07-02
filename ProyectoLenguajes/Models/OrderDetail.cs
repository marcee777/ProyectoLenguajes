using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Models
{
    public class OrderDetail
    {
        [Required]
        [Column(Order = 0)]
        [ForeignKey(nameof(Dish))]
        public int DishId { get; set; }

        public Dish Dish { get; set; }

        [Required]
        [Column(Order = 1)]
        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }

        public Order Order { get; set; }

        [Required]
        public int Amount { get; set; }

    }
}
