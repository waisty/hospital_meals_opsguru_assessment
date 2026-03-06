using Hospital.Auth.Core.InternalModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hospital.Auth.Core.Implementation
{
    /// <summary>
    /// Seeds initial users when the database is empty or when SeedData:Enabled is true.
    /// Duplicate or failed inserts are logged as warnings and do not stop seeding.
    /// </summary>
    internal sealed class AuthSeedDataHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthSeedDataHostedService> _logger;

        public AuthSeedDataHostedService(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<AuthSeedDataHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
        }


        /// <summary>
        /// Returns seed users with their plain-text password (used only to compute PasswordHash before insert).
        /// </summary>
        private static List<(User user, string password)> GetSeedUsers() => new()
        {
            (new User { Username = "admin", Name = "Administrator", Admin = true, PatientAdmin = false, MealsAdmin = false, MealsUser = false, KitchenUser = false }, "Admin123!"),
            (new User { Username = "mealsuser", Name = "Meals User", Admin = false, PatientAdmin = false, MealsAdmin = false, MealsUser = true, KitchenUser = false }, "User123!"),
        };

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var enabledByConfig = string.Equals(
                _configuration["SeedData:Enabled"],
                "true",
                StringComparison.OrdinalIgnoreCase);

            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthDBContext>();

            bool databaseEmpty = false;
            try
            {
                databaseEmpty = await db.Users.CountAsync(cancellationToken).ConfigureAwait(false) == 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not check if users table is empty; skipping seed.");
                return;
            }

            if (!enabledByConfig && !databaseEmpty)
            {
                _logger.LogDebug("Seed data skipped: database has users and SeedData:Enabled is not set.");
                return;
            }

            var seedUsers = GetSeedUsers();
            foreach (var (user, password) in seedUsers)
            {
                try
                {
                    string? passwordHash = await db.Database
                        .SqlQueryRaw<string>("SELECT crypt({0}, gen_salt('bf')) AS \"Value\"", password)
                        .FirstOrDefaultAsync(cancellationToken)
                        .ConfigureAwait(false);

                    if (string.IsNullOrEmpty(passwordHash))
                    {
                        _logger.LogWarning("Could not compute password hash for user {Username}; skipping.", user.Username);
                        continue;
                    }

                    user.PasswordHash = passwordHash;
                    db.Users.Add(user);
                    await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation("Seed user added: {Username}", user.Username);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Seed insert failed for user {Username}; continuing with next.", user.Username);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    }
}
