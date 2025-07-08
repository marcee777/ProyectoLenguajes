using System.Linq.Expressions;

namespace ProyectoLenguajes.Data.Repository.Interfaces
{
    /**
     * Interfaz genérica IRepository<T>
     * 
     * Define las operaciones básicas de un repositorio para una entidad genérica T.
     * Permite agregar, obtener (con filtro y propiedades relacionadas opcionales),
     * obtener todas las entidades, eliminar una entidad o un conjunto de entidades.
     * Esta interfaz es la base para implementar el patrón Repository y facilitar la
     * abstracción del acceso a datos en la aplicación.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodríguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

    public interface IRepository<T> where T : class
    {
        void Add(T entity);

        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);

        IEnumerable<T> GetAll(string? includeProperties = null);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);
    }
}
