using System.ComponentModel.DataAnnotations;

namespace ProyectoLenguajes.Models.ApiModels
{
    /**
     * Clase LoginDto
     * 
     * Define un objeto de transferencia de datos (DTO) utilizado para 
     * capturar la información necesaria durante el proceso de autenticación 
     * de un usuario. Contiene los campos requeridos de correo electrónico 
     * y contraseña, los cuales son validados mediante anotaciones de datos.
     * 
     * Esta clase se emplea en el endpoint de inicio de sesión de la API 
     * para autenticar usuarios de manera segura.
     * 
     * @author Melanie Arce C30634  
     * @author Carolina Rodríguez C36640  
     * @author Marcela Rojas C36975  
     * @version 07/07/25
     */

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
