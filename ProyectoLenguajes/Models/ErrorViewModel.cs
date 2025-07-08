namespace ProyectoLenguajes.Models
{
    /**
     * Clase ErrorViewModel
     * 
     * Modelo simple para manejar informaci�n de errores en las vistas,
     * contiene el Id de la solicitud para prop�sitos de diagn�stico
     * y una propiedad que indica si debe mostrarse dicho Id.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodriguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
