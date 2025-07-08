namespace ProyectoLenguajes.Models.ApiModels
{
    /**
     * Clase OrderDetailDto
     * 
     * Representa un Data Transfer Object (DTO) para los detalles de una orden, 
     * incluyendo información relevante como el identificador del plato, 
     * su nombre, precio y la cantidad solicitada. 
     * 
     * Este DTO se utiliza para transportar datos de los detalles de una orden 
     * desde el backend hacia el frontend o cliente API, facilitando la visualización 
     * y el procesamiento de la información de cada ítem en una orden.
     * 
     * @author Melanie Arce C30634  
     * @author Carolina Rodríguez C36640  
     * @author Marcela Rojas C36975  
     * @version 07/07/25
     */

    public class OrderDetailDto
    {
        public int DishId { get; set; }
        public string DishName { get; set; }   // útil para mostrar
        public decimal DishPrice { get; set; } // útil para mostrar
        public int Amount { get; set; }
    }
}
