using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models;
using ProyectoPedidosExpress.Models;

namespace ProyectoLenguajes.Data.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private ApplicationDbContext _db;
        public OrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Order order)
        {
            _db.Orders.Update(order);
        }
    }
}
