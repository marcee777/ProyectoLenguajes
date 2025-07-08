using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Data.Repository.Interfaces
{
    /**
     * Interfaz IStatusRepository
     * 
     * Define las operaciones específicas para la entidad Status,
     * extendiendo la funcionalidad básica del repositorio genérico IRepository.
     * Incluye el método Update para actualizar un objeto Status en la base de datos.
     * Forma parte del patrón Repository para la gestión de datos en la aplicación.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodríguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

    public interface IStatusRepository : IRepository<Status>
    {
        void Update(Status status); 
    }
}
