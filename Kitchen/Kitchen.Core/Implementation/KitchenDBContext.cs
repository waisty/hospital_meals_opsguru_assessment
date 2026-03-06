using Hospital.Kitchen.Core.InternalModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hospital.Kitchen.Core.Implementation
{
    internal class KitchenDBContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public KitchenDBContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<Tray> Trays { get; set; }
        public DbSet<TrayIngredient> TrayIngredients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration["ConnectionString"]);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Entity<Tray>(Tray.Configure);
            modelBuilder.Entity<TrayIngredient>(TrayIngredient.Configure);
        }
    }
}
