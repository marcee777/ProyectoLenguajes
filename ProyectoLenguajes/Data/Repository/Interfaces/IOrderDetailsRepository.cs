using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Data.Repository.Interfaces
{
    public interface IOrderDetailsRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail orderDetail);
    }
}
