using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hospital.Auth.Core.Implementation
{
    /// <summary>
    /// Applies pending EF Core migrations to the auth database when the application starts.
    /// </summary>
    internal sealed class AuthDbMigrationHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuthDbMigrationHostedService> _logger;

        public AuthDbMigrationHostedService(IServiceScopeFactory scopeFactory, ILogger<AuthDbMigrationHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Applying auth database migrations...");
            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthDBContext>();
            
            await db.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            await db.Database.ExecuteSqlRawAsync("CREATE EXTENSION IF NOT EXISTS pgcrypto;", cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Auth database migrations applied.");
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
