using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProyectoLenguajes.Areas.Admin.Views.ViewModel
{
    public class OrderIndexVM
    {
        public List<OrderListItemVM> Orders { get; set; } = new();

        public IEnumerable<SelectListItem> ClientList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> StatusList { get; set; } = new List<SelectListItem>();

        public string SelectedClientId { get; set; }
        public int? SelectedStatusId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
