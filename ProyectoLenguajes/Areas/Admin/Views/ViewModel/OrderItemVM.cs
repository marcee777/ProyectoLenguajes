namespace ProyectoLenguajes.Areas.Admin.Views.ViewModel
{
    public class OrderListItemVM
    {
        public int OrderId { get; set; }
        public string ClientName { get; set; }
        public string StatusName { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderDishItemVM> Items { get; set; } = new();
    }
}
