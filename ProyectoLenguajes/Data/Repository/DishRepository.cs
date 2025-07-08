using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Data.Repository
{
    /**
     * Clase DishRepository
     * 
     * Implementa el repositorio específico para la entidad Dish, extendiendo la funcionalidad
     * básica del repositorio genérico Repository<Dish> y la interfaz IDishRepository.
     * Proporciona métodos para manejar las operaciones CRUD sobre los platos (Dish),
     * incluyendo una implementación para actualizar un plato existente en la base de datos.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodríguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

    public class DishRepository : Repository<Dish>, IDishRepository
    {
        private ApplicationDbContext _db;

        public DishRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Dish dish)
        {
            _db.Dishes.Update(dish);
        }
    }
}
