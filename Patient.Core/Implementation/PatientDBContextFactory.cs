using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Patient.Core.Implementation
{
    /// <summary>
    /// Used by EF Core tools at design time (e.g. migrations) when no startup project is available.
    /// </summary>
    internal sealed class PatientDBContextFactory : IDesignTimeDbContextFactory<PatientDBContext>
    {
        public PatientDBContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["PatientConnectionString"] = Environment.GetEnvironmentVariable("PatientConnectionString")
                        ?? "Host=localhost;Database=hospitalmeals_patient_db;Username=postgres;Password=postgres"
                })
                .Build();

            return new PatientDBContext(configuration);
        }
    }
}
