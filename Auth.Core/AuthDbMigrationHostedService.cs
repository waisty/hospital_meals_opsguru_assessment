using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Auth.Core.Implementation
{
    /// <summary>
    /// Applies pending EF Core migrations to the auth database when the application starts.
    /// </summary>
    internal sealed class AuthDbMigrationHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AuthDbMigrationHostedService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthDBContext>();
            await db.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
