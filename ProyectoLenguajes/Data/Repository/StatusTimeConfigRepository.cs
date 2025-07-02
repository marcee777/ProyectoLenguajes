using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Data.Repository
{
    public class StatusTimeConfigRepository : Repository<StatusTimeConfig>, IStatusTimeConfigRepository
    {

        private ApplicationDbContext _db;
        public StatusTimeConfigRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(StatusTimeConfig statusTimeConfig)
        {
            _db.StatusTimeConfigs.Update(statusTimeConfig);
        }
    }
}