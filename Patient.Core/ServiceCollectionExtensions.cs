using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Patient.Core.Implementation;

namespace Patient.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPatientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PatientDBContext>();
            services.AddHostedService<PatientDbMigrationHostedService>();
            return services;
        }
    }
}
