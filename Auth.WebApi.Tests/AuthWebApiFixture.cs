using Auth.Core.MockImplementation;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Auth.WebApi.Tests;

/// <summary>
/// WebApplicationFactory that runs the Auth WebApi in "Testing" environment with MockAuthRepo and real AuthHandler.
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
        return base.CreateHost(builder);
    }

    /// <summary>
    /// Gets the singleton MockAuthRepo so tests can add users / valid logins.
    /// </summary>
    public MockAuthRepo MockAuthRepo => Services.GetRequiredService<MockAuthRepo>();
}
