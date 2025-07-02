namespace ProyectoLenguajes.Areas.Admin.Views.ViewModel
{
    public class OrderVM
    {
        public int OrderId { get; set; }
        public string ClientName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string StatusName { get; set; }
        public List<OrderItemVM> Items { get; set; }
    }
}
