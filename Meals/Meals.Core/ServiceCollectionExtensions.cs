using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Hospital.Meals.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers meals-related services (DbContext, migration hosted service, and seed data
        /// hosted service). Use ConnectionString in configuration for the meals database.
        /// Seed runs when the database is empty or when SeedData:Enabled is true.
        /// </summary>
        public static IServiceCollection AddMealsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MealsDBContext>();
            services.AddScoped<IMealsRepo, MealsRepo>();
            services.AddScoped<IMealsHandler, MealsHandler>();
            services.AddSingleton<DelegatingHandler, MealsServiceTokenHandler>();
            services.AddHttpClient<IPatientApiClient, PatientApiClient>(client =>
            {
                var baseUrl = configuration["PatientAPIEndpoint"] ?? throw new InvalidOperationException("PatientAPIEndpoint not configured.");
                client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
                client.Timeout = TimeSpan.FromHours(6);
            }).ConfigureAdditionalHttpMessageHandlers((handlers, sp) =>
            {
                handlers.Add(new MealsServiceTokenHandler(configuration));
            });
            services.AddHttpClient<IKitchenApiClient, KitchenApiClient>(client =>
            {
                var baseUrl = configuration["KitchenAPIEndpoint"] ?? throw new InvalidOperationException("KitchenAPIEndpoint not configured.");
                client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
                client.Timeout = TimeSpan.FromHours(6);
            }).ConfigureAdditionalHttpMessageHandlers((handlers, sp) =>
            {
                handlers.Add(new MealsServiceTokenHandler(configuration));
            });
            services.AddHostedService<MealsDbMigrationHostedService>();
            services.AddHostedService<MealsSeedDataHostedService>();
            return services;
        }
    }

    internal sealed class MealsServiceTokenHandler : DelegatingHandler
    {
        private readonly IConfiguration _configuration;

        public MealsServiceTokenHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = GenerateJwtToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        private string GenerateJwtToken()
        {
            var claims = new[] { new Claim(ClaimIds.mealsServiceClaim, "True") };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured.")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = creds,
                Issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer not configured."),
                Audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience not configured.")
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
