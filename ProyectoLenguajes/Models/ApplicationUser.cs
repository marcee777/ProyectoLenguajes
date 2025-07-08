using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ProyectoLenguajes.Models
{
    /**
     * Clase ApplicationUser
     * 
     * Extiende la clase IdentityUser para agregar propiedades personalizadas del usuario
     * como FirstName, LastName y Address. Estas propiedades permiten almacenar información
     * adicional del usuario dentro del sistema, siendo Address opcional y generalmente usada para clientes.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodríguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(250)]
        public string? Address { get; set; } // Solo se llena si es cliente
    }
}