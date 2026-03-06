using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Patient.Core.Contracts;
using Patient.Core.Implementation;

namespace Patient.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPatientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PatientDBContext>();
            services.AddScoped<IPatientRepo, PatientRepo>();
            services.AddScoped<IPatientHandler, PatientHandler>();
            services.AddHostedService<PatientDbMigrationHostedService>();
            return services;
        }
    }
}
