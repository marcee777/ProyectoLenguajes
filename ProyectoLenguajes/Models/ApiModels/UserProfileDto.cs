namespace ProyectoLenguajes.Models.ApiModels
{
    public class UserProfileDto
    {
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Address { get; set; }
    }
}
