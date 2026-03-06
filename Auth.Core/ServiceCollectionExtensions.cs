using Auth.Core.Contracts;
using Auth.Core.MockImplementation;
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

        /// <summary>
        /// Registers mock Auth implementations for testing (no database, no migrations, no seed).
        /// Use MockAuthRepo and MockAuthHandler to control authentication behavior in tests.
        /// </summary>
        public static IServiceCollection AddMockAuthServicesForTesting(this IServiceCollection services)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddSingleton<MockAuthRepo>();
            services.AddDbContext<AuthDBContext>();
            services.AddScoped<IAuthHandler, AuthHandler>();
            return services;
        }
    }
}
