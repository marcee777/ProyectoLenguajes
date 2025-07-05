using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoLenguajes.Models;
using ProyectoLenguajes.Utilities;

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
                new Status { Id = 1, Name = StaticValues.Status_OnTime, TimeToNextStatus = 10, NextStatusId = 2 },
                new Status { Id = 2, Name = StaticValues.Status_OverTime, TimeToNextStatus = 15, NextStatusId = 3 },
                new Status { Id = 3, Name = StaticValues.Status_Delayed, TimeToNextStatus = null, NextStatusId = null },
                new Status { Id = 4, Name = StaticValues.Status_Canceled, TimeToNextStatus = null, NextStatusId = null },
                new Status { Id = 5, Name = StaticValues.Status_Delivered, TimeToNextStatus = null, NextStatusId = null }
            );
        }
    }
}
