using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoLenguajes.Models;
using ProyectoPedidosExpress.Models;

namespace ProyectoLenguajes.Data
{
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
                new Status { Id = 1, Name = "On Time" },
                new Status { Id = 2, Name = "Over Time" },
                new Status { Id = 3, Name = "Delayed" },
                new Status { Id = 4, Name = "Canceled" },
                new Status { Id = 5, Name = "Delivered" }
            );
        }
    }
}
