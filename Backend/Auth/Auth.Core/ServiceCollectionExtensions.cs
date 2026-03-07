using Hospital.Auth.Core.Contracts;
using Hospital.Auth.Core.MockImplementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hospital.Auth.Core.Implementation
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers Auth services (DbContext, repo, handler) and a hosted service that applies
        /// pending EF Core migrations when the application starts.
        /// </summary>
        public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddDbContext<AuthDBContext>();
            services.AddScoped<IAuthRepo, AuthRepo>();
            services.AddScoped<IAuthHandler, AuthHandler>();
            services.AddHostedService<AuthDbMigrationHostedService>();
            services.AddHostedService<AuthSeedDataHostedService>();
            return services;
        }

        /// <summary>
        /// Registers mock Auth implementations for testing (no database, no migrations, no seed).
        /// Use MockAuthRepo to control which username/password pairs are valid; real AuthHandler generates JWTs.
        /// </summary>
        public static IServiceCollection AddMockAuthServicesForTesting(this IServiceCollection services)
        {
            services.AddSingleton<MockAuthRepo>();
            services.AddScoped<IAuthRepo>(sp => sp.GetRequiredService<MockAuthRepo>());
            services.AddScoped<IAuthHandler, AuthHandler>();
            return services;
        }
    }
}
