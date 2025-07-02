using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Data.Repository.Interfaces
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail orderDetail);
    }
}
