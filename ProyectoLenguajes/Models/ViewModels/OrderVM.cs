using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoLenguajes.Models.ViewModels
{
    /**
     * Clase OrderVM
     * 
     * ViewModel que encapsula los datos necesarios para las vistas relacionadas con pedidos.
     * Contiene la entidad Order, la lista de detalles del pedido (OrderDetails), 
     * y una colección de elementos para selección de platillos (DishList) usada en la interfaz de usuario, como en dropdowns.
     * Se utilizan atributos [ValidateNever] para evitar validaciones automáticas en estas propiedades al enlazar modelos.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodríguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

    public class OrderVM
    {
        [ValidateNever]
        public Order Order { get; set; }

        [ValidateNever]
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        // Para selección de platillos en la vista (ej. dropdown)
        [ValidateNever]
        public IEnumerable<SelectListItem> DishList { get; set; }

    }
}
