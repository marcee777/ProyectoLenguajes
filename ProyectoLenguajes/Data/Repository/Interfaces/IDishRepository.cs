using ProyectoPedidosExpress.Models;

namespace ProyectoLenguajes.Data.Repository.Interfaces
{
    public interface IDishRepository : IRepository<Dish>
    {
        void Update(Dish dish);
    }
}
