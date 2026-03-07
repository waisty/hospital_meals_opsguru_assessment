using Hospital.Auth.Core.Implementation;
using Hospital.Auth.Core.MockImplementation;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hospital.Auth.WebApi.Tests;

/// <summary>
/// WebApplicationFactory that runs the Auth WebApi in "Testing" environment with MockAuthRepo and real AuthHandler.
/// Matches the Meals pattern: fixture owns test service registration and exposes ClearAll() for isolation.
/// </summary>
public sealed class AuthWebApiFixture : WebApplicationFactory<Program>
{
    private const string TestJwtKey = "test-signing-key-at-least-32-characters-long";

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = TestJwtKey,
                ["Jwt:Issuer"] = "Auth.WebApi.Tests",
                ["Jwt:Audience"] = "Auth.WebApi.Tests"
            });
        });

        builder.ConfigureServices((_, services) =>
        {
            services.AddMockAuthServicesForTesting();
        });

        return base.CreateHost(builder);
    }

    /// <summary>
    /// Gets the singleton MockAuthRepo so tests can add users / valid logins.
    /// </summary>
    public MockAuthRepo MockAuthRepo => Services.GetRequiredService<MockAuthRepo>();

    /// <summary>
    /// Clears all mock data for test isolation. Call from test class constructors.
    /// </summary>
    public void ClearAll()
    {
        MockAuthRepo.Clear();
    }
}
