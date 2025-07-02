using System.ComponentModel.DataAnnotations;

namespace ProyectoLenguajes.Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

    }
}
