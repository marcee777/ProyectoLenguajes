using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Data.Repository
{
    /**
     * Clase OrderDetailRepository
     * 
     * Implementa el repositorio específico para la entidad OrderDetail, extendiendo la funcionalidad
     * del repositorio genérico Repository<OrderDetail> y la interfaz IOrderDetailRepository.
     * Proporciona métodos para manejar las operaciones CRUD sobre los detalles de órdenes,
     * incluyendo una implementación para actualizar un detalle de orden en la base de datos.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodríguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

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
