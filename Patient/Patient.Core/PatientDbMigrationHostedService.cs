using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Hospital.Patient.Core.Implementation;

namespace Hospital.Patient.Core
{
    /// <summary>
    /// Applies pending EF Core migrations to the patient database when the application starts.
    /// </summary>
    internal sealed class PatientDbMigrationHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PatientDbMigrationHostedService> _logger;

        public PatientDbMigrationHostedService(IServiceScopeFactory scopeFactory, ILogger<PatientDbMigrationHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Applying patient database migrations...");
            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<PatientDBContext>();
            await db.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Patient database migrations applied.");
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
