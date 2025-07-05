namespace ProyectoLenguajes.Models.ApiModels
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; } // solo si se desea mostrarlo
        public DateTime CreatedAt { get; set; }
        public DateTime LastStatusChange { get; set; }
        public StatusDto Status { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
    }
}
