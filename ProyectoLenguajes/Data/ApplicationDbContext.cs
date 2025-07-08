using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoLenguajes.Models;
using ProyectoLenguajes.Utilities;

namespace ProyectoLenguajes.Data
{
    /**
     * Clase ApplicationDbContext
     * 
     * Representa el contexto de la base de datos principal de la aplicación, extendiendo 
     * de IdentityDbContext para integrar la gestión de usuarios con ASP.NET Identity.
     * Define los DbSet que representan las entidades del dominio como Orders, OrderDetails, 
     * Dishes, ApplicationUsers y Status, y configura relaciones y datos iniciales mediante 
     * el método OnModelCreating.
     * 
     * También establece una clave primaria compuesta para la entidad OrderDetail y 
     * precarga los estados del ciclo de vida de una orden utilizando valores definidos 
     * en la clase estática StaticValues.
     * 
     * Esta clase permite la interacción fluida con la base de datos mediante Entity Framework Core.
     * 
     * @author Melanie Arce C30634  
     * @author Carolina Rodríguez C36640  
     * @author Marcela Rojas C36975  
     * @version 07/07/25
     */

    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Status> Status { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Clave primaria compuesta para OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderId, od.DishId });

            // Semilla para estados
            modelBuilder.Entity<Status>().HasData(
                new Status { Id = 1, Name = StaticValues.Status_Unconfirmed, TimeToNextStatus = null, NextStatusId = 2 },
                new Status { Id = 2, Name = StaticValues.Status_OnTime, TimeToNextStatus = 10, NextStatusId = 3 },
                new Status { Id = 3, Name = StaticValues.Status_OverTime, TimeToNextStatus = 15, NextStatusId = 4 },
                new Status { Id = 4, Name = StaticValues.Status_Delayed, TimeToNextStatus = null, NextStatusId = null },
                new Status { Id = 5, Name = StaticValues.Status_Canceled, TimeToNextStatus = null, NextStatusId = null },
                new Status { Id = 6, Name = StaticValues.Status_Delivered, TimeToNextStatus = null, NextStatusId = null }
            );
        }
    }
}
