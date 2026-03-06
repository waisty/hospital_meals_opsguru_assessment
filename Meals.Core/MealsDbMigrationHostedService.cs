using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hospital.Meals.Core.Implementation
{
    /// <summary>
    /// Applies pending EF Core migrations to the meals database when the application starts.
    /// </summary>
    internal sealed class MealsDbMigrationHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MealsDbMigrationHostedService> _logger;

        public MealsDbMigrationHostedService(IServiceScopeFactory scopeFactory, ILogger<MealsDbMigrationHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Applying meals database migrations...");
            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<MealsDBContext>();
            await db.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Meals database migrations applied.");
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
