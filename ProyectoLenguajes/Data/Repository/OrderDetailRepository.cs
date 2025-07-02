using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models;
using ProyectoPedidosExpress.Models;

namespace ProyectoLenguajes.Data.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private ApplicationDbContext _db;
        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderDetail orderDetail)
        {
            _db.OrderDetails.Update(orderDetail);
        }
    }
}
