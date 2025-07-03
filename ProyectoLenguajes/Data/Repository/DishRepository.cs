using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Data.Repository
{
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
