using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hospital.Patient.Core.Contracts;
using Hospital.Patient.WebApi.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Hospital.Patient.WebApi.Tests;

public sealed class PatientWebApiFixture : WebApplicationFactory<Program>
{
    private const string TestJwtKey = "test-signing-key-at-least-32-characters-long";
    private const string TestIssuer = "Patient.WebApi.Tests";
    private const string TestAudience = "Patient.WebApi.Tests";

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = TestJwtKey,
                ["Jwt:Issuer"] = TestIssuer,
                ["Jwt:Audience"] = TestAudience,
            });
        });

        builder.ConfigureServices((context, services) =>
        {
            services.AddJwtAuthentication(context.Configuration);
            services.AddSingleton<MockPatientHandler>();
            services.AddScoped<IPatientHandler>(sp => sp.GetRequiredService<MockPatientHandler>());
            services.AddScoped<IReferenceDataHandler>(sp => sp.GetRequiredService<MockPatientHandler>());
        });

        return base.CreateHost(builder);
    }

    public MockPatientHandler MockHandler => Services.GetRequiredService<MockPatientHandler>();

    /// <summary>
    /// Clears all mock data for test isolation. Call from test class constructors.
    /// </summary>
    public void ClearAll()
    {
        MockHandler.Clear();
    }

    public HttpClient CreateAuthenticatedClient(params string[] claimTypes)
    {
        var client = CreateClient();
        var token = GenerateTestToken(claimTypes);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static string GenerateTestToken(params string[] claimTypes)
    {
        var claims = claimTypes.Select(c => new Claim(c, "True")).ToList();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestJwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = creds,
            Issuer = TestIssuer,
            Audience = TestAudience
        };
        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(descriptor));
    }
}
