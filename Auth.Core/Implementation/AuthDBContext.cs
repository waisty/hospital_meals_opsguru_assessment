using Hospital.Auth.Core.InternalModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hospital.Auth.Core.Implementation
{
    internal class AuthDBContext : DbContext
    {
        private readonly IConfiguration configuration;

        public AuthDBContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(this.configuration["ConnectionString"]);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Entity<User>(User.Configure);
        }
    }
}
