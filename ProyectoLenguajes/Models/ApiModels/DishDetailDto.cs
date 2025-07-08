namespace ProyectoLenguajes.Models.ApiModels
{
    /**
     * Clase DishDetailDto
     * 
     * Representa un Data Transfer Object (DTO) que se utiliza para transferir los detalles 
     * completos de un plato (Dish) a través de la API. Contiene propiedades básicas como 
     * el identificador, nombre, descripción, precio y URL de la imagen asociada al plato.
     * 
     * Este DTO es útil para desacoplar la capa de presentación de la entidad original del modelo 
     * y controlar de forma segura los datos expuestos al cliente.
     * 
     * @author Melanie Arce C30634  
     * @author Carolina Rodríguez C36640  
     * @author Marcela Rojas C36975  
     * @version 07/07/25
     */

    public class DishDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string URLImage { get; set; }
    }
}
