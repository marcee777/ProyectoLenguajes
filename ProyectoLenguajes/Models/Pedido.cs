using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoLenguajes.Models
{
    public class Pedido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }



        [Required]
        public DateTime FechaCreacion { get; set; }

        [ForeignKey("EstadoId")]
        public EstadoPedido Estado { get; set; }


    }
}
