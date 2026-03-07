using Hospital.Kitchen.Core.Contracts;
using Hospital.Kitchen.Core.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Kitchen.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers Kitchen-related services (DbContext, repo, handler), migration hosted service,
        /// and seed data hosted service. Use ConnectionString in configuration for the Kitchen database.
        /// Seed runs when the database is empty or when SeedData:Enabled is true.
        /// </summary>
        public static IServiceCollection AddKitchenServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<KitchenDBContext>();
            services.AddScoped<IKitchenRepo, KitchenRepo>();
            services.AddScoped<IKitchenHandler, KitchenHandler>();
            services.AddHostedService<KitchenDbMigrationHostedService>();
            return services;
        }
    }
}
