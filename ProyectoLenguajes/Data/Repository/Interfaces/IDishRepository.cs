using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Data.Repository.Interfaces
{
    /*
     * Interfaz específica para la entidad Dish que extiende la interfaz genérica IRepository<Dish>,
     * proporcionando una abstracción para el acceso y manipulación de los datos relacionados con los platos.
     * 
     * Define el método Update para actualizar instancias de Dish en la base de datos.
     * Forma parte de la implementación del patrón Repository, lo que permite desacoplar la lógica de acceso
     * a datos de la lógica de negocios. Esta interfaz se utiliza junto con el patrón Unit of Work para
     * asegurar una gestión eficiente y coherente de las transacciones.
     *
     * @author Melanie Arce C30634
     * @author Carolina Rodríguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

    public interface IDishRepository : IRepository<Dish>
    {
        void Update(Dish dish);
    }
}
