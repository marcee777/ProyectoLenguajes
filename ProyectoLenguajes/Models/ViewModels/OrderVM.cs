using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoLenguajes.Models.ViewModels
{
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
