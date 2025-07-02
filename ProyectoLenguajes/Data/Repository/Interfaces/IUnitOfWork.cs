namespace ProyectoLenguajes.Data.Repository.Interfaces
{
    public interface IUnitOfWork
    {
        IDishRepository Dish { get; }
        IOrderDetailRepository OrderDetail { get; }
        IOrderRepository Order { get; } 
        IStatusRepository Status { get; }

        void Save();
    }
}
