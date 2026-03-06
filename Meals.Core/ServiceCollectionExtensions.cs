using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hospital.Meals.Core.Implementation;

namespace Hospital.Meals.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers Meals DbContext, migration hosted service, and seed data hosted service.
        /// Use ConnectionString in configuration for the meals database.
        /// </summary>
        public static IServiceCollection AddMealsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MealsDBContext>();
            services.AddHostedService<MealsDbMigrationHostedService>();
            services.AddHostedService<MealsSeedDataHostedService>();
            return services;
        }
    }
}
