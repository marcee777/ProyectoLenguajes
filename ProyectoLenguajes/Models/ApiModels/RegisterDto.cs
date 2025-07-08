using System.ComponentModel.DataAnnotations;

namespace ProyectoLenguajes.Models.ApiModels
{
    /**
     * Clase RegisterDto
     * 
     * Define un Data Transfer Object (DTO) utilizado para el proceso de registro de nuevos usuarios 
     * a través de una API. Contiene propiedades anotadas con validaciones que aseguran la integridad 
     * de los datos ingresados por el usuario, como correo electrónico, contraseña, nombre, apellidos 
     * y dirección.
     * 
     * Este DTO facilita la recepción y validación de datos en operaciones de creación de cuenta, 
     * asegurando una estructura clara y segura para el registro.
     * 
     * @author Melanie Arce C30634  
     * @author Carolina Rodríguez C36640  
     * @author Marcela Rojas C36975  
     * @version 07/07/25
     */

    public class RegisterDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, ErrorMessage = "First name must not exceed 100 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, ErrorMessage = "Last name must not exceed 100 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(250, ErrorMessage = "Address must not exceed 250 characters.")]
        public string Address { get; set; }
    }
}
