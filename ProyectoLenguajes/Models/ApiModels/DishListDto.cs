namespace ProyectoLenguajes.Models.ApiModels
{
    /**
     * Clase DishListDto
     * 
     * Define un objeto de transferencia de datos (DTO) que representa 
     * la información resumida de un plato, incluyendo su identificador, 
     * nombre y precio. Se utiliza principalmente para enviar listados 
     * de platos activos a través de la API, optimizando el rendimiento 
     * al evitar el envío de datos innecesarios.
     * 
     * Este DTO es ideal para vistas previas de menús o resultados de búsqueda.
     * 
     * @author Melanie Arce C30634  
     * @author Carolina Rodríguez C36640  
     * @author Marcela Rojas C36975  
     * @version 07/07/25
     */

    public class DishListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
