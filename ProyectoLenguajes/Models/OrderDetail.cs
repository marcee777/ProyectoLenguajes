using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;
using ProyectoLenguajes.Models;
using ProyectoLenguajes.Models.ViewModels;

namespace ProyectoLenguajes.Models
{
    /**
     * Clase OrderDetail
     * 
     * Representa el detalle de una orden, relacionando un plato (Dish) específico con una orden (Order).
     * Incluye la cantidad solicitada del plato y utiliza claves foráneas para establecer las relaciones.
     * Contiene validaciones para asegurar que la cantidad sea al menos 1.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodriguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

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
        //validación de rango para evitar valores negativos o cero
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be at least 1")]
        public int Amount { get; set; }

    }
}