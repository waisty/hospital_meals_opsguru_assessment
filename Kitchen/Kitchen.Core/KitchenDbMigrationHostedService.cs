using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hospital.Kitchen.Core.Implementation
{
    /// <summary>
    /// Applies pending EF Core migrations to the Kitchen database when the application starts.
    /// </summary>
    internal sealed class KitchenDbMigrationHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<KitchenDbMigrationHostedService> _logger;

        public KitchenDbMigrationHostedService(IServiceScopeFactory scopeFactory, ILogger<KitchenDbMigrationHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Applying Kitchen database migrations...");
            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<KitchenDBContext>();
            await db.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Kitchen database migrations applied.");
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
