using Auth.Core.Contracts;
using Core.Auth.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebExceptionHandler;

namespace Auth.Core.Implementation
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers Auth services (DbContext, repo, handler) and a hosted service that applies
        /// pending EF Core migrations when the application starts.
        /// </summary>
        public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddDbContext<AuthDBContext>();
            services.AddScoped<IAuthRepo, AuthRepo>();
            services.AddScoped<IAuthHandler, AuthHandler>();
            services.AddHostedService<AuthDbMigrationHostedService>();
            services.AddHostedService<AuthSeedDataHostedService>();
            return services;
        }
    }
}
