using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hospital.Patient.Core.Contracts;
using Hospital.Patient.Core.Implementation;

namespace Hospital.Patient.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers Patient services (DbContext, repo, handler), migration hosted service,
        /// and a hosted service that seeds reference data and sample patients when the database
        /// is empty or when SeedData:Enabled is true.
        /// </summary>
        public static IServiceCollection AddPatientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PatientDBContext>();
            services.AddScoped<IPatientRepo, PatientRepo>();
            services.AddScoped<IPatientHandler, PatientHandler>();
            services.AddHostedService<PatientDbMigrationHostedService>();
            services.AddHostedService<PatientSeedDataHostedService>();
            return services;
        }
    }
}
