using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hospital.Patient.Core.Implementation;

namespace Hospital.Patient.Core
{
    /// <summary>
    /// Applies pending EF Core migrations to the patient database when the application starts.
    /// </summary>
    internal sealed class PatientDbMigrationHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PatientDbMigrationHostedService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<PatientDBContext>();
            await db.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
