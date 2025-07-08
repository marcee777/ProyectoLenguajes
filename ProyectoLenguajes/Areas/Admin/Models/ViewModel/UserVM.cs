using System.ComponentModel.DataAnnotations;

namespace ProyectoLenguajes.Areas.Admin.Models.ViewModels
{

    /**
     * ViewModel que representa la información básica de un usuario para su gestión en el área administrativa.
     * Incluye datos personales, roles asignados y estado de bloqueo.
     * Utilizado para mostrar, editar y validar datos de usuarios en las vistas correspondientes.
     * 
     * Las validaciones de datos se aplican mediante atributos de DataAnnotations para asegurar la integridad de la información.
     * 
     * @author: Melanie Arce C30634
     * @author: Carolina Rodríguez C36640
     * @author: Marcela Rojas C36975
     * @version: 07/07/25
     */
    public class UserVM
    {
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [StringLength(250)]
        public string? Address { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();

        public bool IsBlocked { get; set; }
    }
}
