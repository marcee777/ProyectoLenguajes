using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Data.Repository
{
    /**
     * Clase StatusRepository
     * 
     * Implementa el repositorio específico para la entidad Status, extendiendo la funcionalidad
     * genérica del repositorio base Repository<Status> e implementando la interfaz IStatusRepository.
     * Proporciona la operación de actualización (Update) para el estado, utilizando Entity Framework Core.
     * 
     * Forma parte del patrón Repository para una mejor abstracción y manejo del acceso a datos en la aplicación.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodríguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

    public class StatusRepository : Repository<Status>, IStatusRepository
    {
        private ApplicationDbContext _db;

        public StatusRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Status status)
        {
            _db.Status.Update(status);
        }
    }
}
