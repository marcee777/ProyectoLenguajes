namespace ProyectoLenguajes.Models.ApiModels
{
    public class OrderDetailDto
    {
        public int DishId { get; set; }
        public string DishName { get; set; }   // útil para mostrar
        public decimal DishPrice { get; set; } // útil para mostrar
        public int Amount { get; set; }
    }
}
