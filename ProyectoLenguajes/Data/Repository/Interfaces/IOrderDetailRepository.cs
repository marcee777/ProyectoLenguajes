using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Data.Repository.Interfaces
{
    /**
     * Interfaz IOrderDetailRepository
     * 
     * Esta interfaz extiende IRepository para la entidad OrderDetail, permitiendo
     * la implementación de métodos específicos como Update. Forma parte del patrón
     * Repository y se utiliza junto con Unit of Work para una gestión eficiente
     * y desacoplada de los datos.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodríguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail orderDetail);
    }
}
