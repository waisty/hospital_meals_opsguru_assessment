using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Patient.Core.Implementation;

namespace Patient.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers Patient services (DbContext) and a hosted service that applies
        /// pending EF Core migrations when the application starts.
        /// Uses its own database via PatientConnectionString.
        /// </summary>
        public static IServiceCollection AddPatientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PatientDBContext>();
            services.AddHostedService<PatientDbMigrationHostedService>();
            return services;
        }
    }
}
