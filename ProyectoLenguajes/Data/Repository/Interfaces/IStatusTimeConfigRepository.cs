using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Data.Repository.Interfaces
{
    public interface IStatusTimeConfigRepository : IRepository<StatusTimeConfig>
    {
        void Update(StatusTimeConfig statusTimeConfig);

    }
}
