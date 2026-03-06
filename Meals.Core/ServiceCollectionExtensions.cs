using Hospital.Meals.Core.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Meals.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers meals-related services (DbContext and a hosted service that applies
        /// pending EF Core migrations when the application starts).
        /// Use ConnectionString in configuration for the meals database.
        /// </summary>
        public static IServiceCollection AddMealsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MealsDBContext>();
            services.AddHostedService<MealsDbMigrationHostedService>();
            return services;
        }
    }
}
