using ProyectoLenguajes.Data.Repository.Interfaces;

namespace ProyectoLenguajes.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            
        }

        public void Save()
        {
            _db.SaveChanges();

        }
    }
}
