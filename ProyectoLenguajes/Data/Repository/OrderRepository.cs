using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Data.Repository
{
    /**
     * Clase OrderRepository
     * 
     * Implementa el repositorio específico para la entidad Order, derivando del repositorio genérico
     * Repository<Order> y cumpliendo con la interfaz IOrderRepository. Esta clase facilita
     * la gestión de las órdenes en la base de datos, incluyendo la funcionalidad para actualizar
     * los registros de órdenes mediante el método Update.
     * 
     * Se utiliza dentro del patrón Repository para abstraer la lógica de acceso a datos,
     * mejorando la mantenibilidad y escalabilidad del proyecto.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodríguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

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
