using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProyectoLenguajes.Data.Repository.Interfaces;


namespace ProyectoLenguajes.Data.Repository
{
    /**
     * Clase genérica Repository<T>
     * 
     * Implementa la interfaz IRepository<T> para proveer operaciones básicas de acceso a datos
     * (CRUD) para cualquier entidad del tipo T. Esta clase utiliza Entity Framework Core para
     * interactuar con la base de datos a través de un DbSet<T>, permitiendo agregar, obtener,
     * eliminar y listar entidades, además de manejar propiedades relacionadas mediante inclusión
     * de navegación (includeProperties).
     * 
     * Esta implementación es parte del patrón Repository, que facilita la abstracción y reutilización
     * del acceso a datos, mejorando la organización y mantenibilidad del código.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodríguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null) //category,obj1,obj2,,
        {
            IQueryable<T> query = dbSet;


            if (includeProperties != null)
            {
                foreach (var p in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(p);
                }
            }

            query = query.Where(filter);
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (includeProperties != null)
            {
                foreach (var p in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(p);
                }
            }

            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

    }
}
