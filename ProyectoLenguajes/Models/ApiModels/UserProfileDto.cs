namespace ProyectoLenguajes.Models.ApiModels
{
    /**
     * Clase UserProfileDto
     * 
     * Representa un Data Transfer Object (DTO) para enviar la información del perfil de un usuario.
     * Contiene propiedades para el correo electrónico, nombre, apellido y dirección del usuario.
     * Esta clase es utilizada para exponer datos del perfil de usuario de forma segura y estructurada en las respuestas de la API.
     * 
     * @author Melanie Arce C30634  
     * @author Carolina Rodríguez C36640  
     * @author Marcela Rojas C36975  
     * @version 07/07/25
     */

    public class UserProfileDto
    {
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Address { get; set; }
    }
}
