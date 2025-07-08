namespace ProyectoLenguajes.Models.ApiModels
{
    /**
     * Clase OrderDto
     * 
     * Representa un Data Transfer Object (DTO) para una orden, encapsulando 
     * información esencial como el identificador de la orden, el cliente que la generó, 
     * fechas relevantes (creación y último cambio de estado), el estado actual de la orden 
     * y los detalles de los productos incluidos.
     * 
     * Esta clase permite transportar de manera eficiente los datos de una orden completa 
     * desde el servidor hacia el cliente o interfaz de usuario en contextos de API o vistas.
     * 
     * @author Melanie Arce C30634  
     * @author Carolina Rodríguez C36640  
     * @author Marcela Rojas C36975  
     * @version 07/07/25
     */

    public class OrderDto
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime LastStatusChange { get; set; }
        public StatusDto Status { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
    }
}
