using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Data.Repository.Interfaces
{
    /**
     * Interfaz IOrderRepository
     * 
     * Esta interfaz extiende IRepository para la entidad Order, proporcionando
     * un contrato para operaciones específicas como Update. Forma parte del patrón
     * Repository y se utiliza en conjunto con Unit of Work para manejar las
     * operaciones de persistencia de datos de manera desacoplada y eficiente.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodríguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

    public interface IOrderRepository : IRepository<Order>
    {
        void Update(Order order);

    }
}
