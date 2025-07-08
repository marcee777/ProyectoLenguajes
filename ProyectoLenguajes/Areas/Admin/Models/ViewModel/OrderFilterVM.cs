using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Areas.Admin.Models.ViewModel
{
    /**
     * ViewModel utilizado para filtrar las órdenes en la vista del administrador.
     * Contiene los criterios de búsqueda (cliente, estado, fechas) y las listas de selección necesarias para los filtros,
     * así como la lista resultante de órdenes que cumplen con dichos criterios.
     * 
     * Esta clase facilita la comunicación entre el controlador y la vista al encapsular toda la información necesaria
     * para aplicar filtros dinámicos en la gestión de pedidos.
     * 
     * @author: Melanie Arce C30634
     * @author: Carolina Rodríguez C36640
     * @author: Marcela Rojas C36975
     * @version: 07/07/25
     */
    public class OrderFilterVM
    {
        public string ClientId { get; set; }
        public int? StatusId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<SelectListItem> Clients { get; set; }
        public List<SelectListItem> Statuses { get; set; }

        public List<Order> Orders { get; set; }
    }
}