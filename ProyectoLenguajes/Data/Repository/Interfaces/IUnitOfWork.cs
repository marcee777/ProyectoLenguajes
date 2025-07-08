namespace ProyectoLenguajes.Data.Repository.Interfaces
{
    /**
     * Interfaz IUnitOfWork
     * 
     * Define una unidad de trabajo que agrupa múltiples repositorios específicos 
     * para facilitar la gestión de transacciones y el acceso coherente a los datos.
     * Proporciona acceso a los repositorios de Dish, OrderDetail, Order y Status,
     * además del método Save para persistir los cambios en la base de datos.
     * Implementa el patrón Unit of Work para un manejo eficiente y consistente de la persistencia.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodríguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

    public interface IUnitOfWork
    {
        IDishRepository Dish { get; }
        IOrderDetailRepository OrderDetail { get; }
        IOrderRepository Order { get; } 
        IStatusRepository Status { get; }
        void Save();
    }
}
