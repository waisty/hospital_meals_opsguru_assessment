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
        /// Registers Kitchen-related services (DbContext, repo, handler).
        /// Use ConnectionString in configuration for the Kitchen database.
        /// </summary>
        public static IServiceCollection AddKitchenServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<KitchenDBContext>();
            services.AddScoped<IKitchenRepo, KitchenRepo>();
            services.AddScoped<IKitchenHandler, KitchenHandler>();
            return services;
        }
    }
}
