using ProyectoLenguajes.Data.Repository.Interfaces;

namespace ProyectoLenguajes.Data.Repository
{
    /**
     * Clase UnitOfWork
     * 
     * Implementa el patrón Unit of Work para coordinar el trabajo de múltiples repositorios, 
     * garantizando que las operaciones sobre la base de datos se realicen de manera consistente 
     * y atómica. Proporciona acceso a los repositorios específicos (Dish, OrderDetail, Order, Status)
     * y un método para guardar los cambios realizados en el contexto de la base de datos.
     * 
     * Esta clase facilita la gestión centralizada de las transacciones y la persistencia de datos.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodríguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

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
