using System.ComponentModel.DataAnnotations;

namespace ProyectoLenguajes.Models.ApiModels
{
    /**
     * Clase UpdateUserDto
     * 
     * Define un Data Transfer Object (DTO) utilizado para actualizar la información de un usuario a través de la API.
     * Incluye campos para modificar el nombre, apellido, dirección y, opcionalmente, una nueva contraseña. 
     * La clase asegura validaciones mediante atributos como [Required] y [StringLength] para mantener la integridad de los datos recibidos.
     * 
     * Es empleada principalmente en operaciones PUT relacionadas con el perfil del usuario.
     * 
     * @author Melanie Arce C30634  
     * @author Carolina Rodríguez C36640  
     * @author Marcela Rojas C36975  
     * @version 07/07/25
     */

    public class UpdateUserDto
    {
       
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = null!;

        [StringLength(250)]
        public string? Address { get; set; }

        [StringLength(100, MinimumLength = 6)]
        public string? NewPassword { get; set; }
    }
}
