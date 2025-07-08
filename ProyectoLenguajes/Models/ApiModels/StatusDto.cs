namespace ProyectoLenguajes.Models.ApiModels
{
    /**
     * Clase StatusDto
     * 
     * Representa un Data Transfer Object (DTO) que encapsula los datos esenciales de un estado (Status) 
     * dentro del sistema. Contiene únicamente el identificador y el nombre del estado, lo cual es útil 
     * para transferencias ligeras de información entre el servidor y el cliente en respuestas de API.
     * 
     * Esta clase se utiliza comúnmente para mostrar o manejar el estado actual de pedidos sin exponer 
     * toda la entidad del modelo.
     * 
     * @author Melanie Arce C30634  
     * @author Carolina Rodríguez C36640  
     * @author Marcela Rojas C36975  
     * @version 07/07/25
     */

    public class StatusDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
