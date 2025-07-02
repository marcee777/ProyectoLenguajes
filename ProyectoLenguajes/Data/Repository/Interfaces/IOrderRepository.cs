using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Data.Repository.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        void Update(Order order);

    }
}
