using ProyectoLenguajes.Data.Repository.Interfaces;

namespace ProyectoLenguajes.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public IDishRepository Dish { get; private set; }

        public IOrderDetailRepository OrderDetail { get; private set; }

        public IOrderRepository Order { get; private set; }

        public IStatusRepository Status { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Dish = new DishRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            Order = new OrderRepository(_db);
            Status = new StatusRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
