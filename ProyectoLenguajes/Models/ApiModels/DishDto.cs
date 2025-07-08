namespace ProyectoLenguajes.Models.ApiModels
{
    /**
     * Clase DishDto
     * 
     * Representa un Data Transfer Object (DTO) simplificado que se utiliza para 
     * transferir información básica de un plato (Dish) a través de la API. 
     * Incluye únicamente el identificador, el nombre y el precio del plato.
     * 
     * Este DTO es especialmente útil para mostrar listados o resúmenes de platos 
     * sin exponer detalles innecesarios, favoreciendo la eficiencia y la seguridad.
     * 
     * @author Melanie Arce C30634  
     * @author Carolina Rodríguez C36640  
     * @author Marcela Rojas C36975  
     * @version 07/07/25
     */

    public class DishDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
