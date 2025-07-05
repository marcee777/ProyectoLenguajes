using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoLenguajes.Models;

namespace ProyectoLenguajes.Areas.Admin.Models.ViewModel
{
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