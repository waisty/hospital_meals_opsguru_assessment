using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Meals.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers meals-related services (DbContext, migration hosted service, and seed data
        /// hosted service). Use ConnectionString in configuration for the meals database.
        /// Seed runs when the database is empty or when SeedData:Enabled is true.
        /// </summary>
        public static IServiceCollection AddMealsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MealsDBContext>();
            services.AddScoped<IMealsRepo, MealsRepo>();
            services.AddScoped<IMealsHandler, MealsHandler>();
            services.AddHostedService<MealsDbMigrationHostedService>();
            services.AddHostedService<MealsSeedDataHostedService>();
            return services;
        }
    }
}
