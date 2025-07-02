using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Data.Repository.Interfaces
{
    public interface IStatusRepository : IRepository<Status>
    {
        void Update(Status status); 
    }
}
