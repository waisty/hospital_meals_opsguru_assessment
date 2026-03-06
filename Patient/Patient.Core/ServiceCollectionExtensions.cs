using Hospital.Patient.Core.Contracts;
using Hospital.Patient.Core.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Hospital.Patient.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers Patient services (DbContext, repo, handler), migration hosted service,
        /// and a hosted service that seeds reference data and sample patients when the database
        /// is empty or when SeedData:Enabled is true.
        /// </summary>
        public static IServiceCollection AddPatientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PatientDBContext>();
            services.AddScoped<IPatientRepo, PatientRepo>();
            services.AddHttpClient<IMealsApiClient, MealsApiClient>(client =>
            {
                var baseUrl = configuration["MealsAPIEndpoint"] ?? throw new Exception("MealsAPIEndpoint not found");
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            }).AddHttpMessageHandler<PatientServiceTokenHandler>(); ;
            services.AddScoped<IPatientHandler, PatientHandler>();
            services.AddHostedService<PatientDbMigrationHostedService>();
            services.AddHostedService<PatientSeedDataHostedService>();
            return services;
        }
    }

    public class PatientServiceTokenHandler : DelegatingHandler
    {
        private IConfiguration configuration;
        public PatientServiceTokenHandler(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 1. Get the token (from cache, identity server, or session)
            var token = GenerateJWTToken();

            // 2. Inject the header
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 3. Continue the request pipeline
            return await base.SendAsync(request, cancellationToken);
        }

        private string GenerateJWTToken()
        {
            Claim[] claims = [new Claim(ClaimIds.patientsServiceClaim, "True")];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new Exception("JWT Key key not found")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = creds,
                Issuer = configuration["Jwt:Issuer"] ?? throw new Exception("JWT Issuer not found"),
                Audience = configuration["Jwt:Audience"] ?? throw new Exception("Jwt Audience not found")
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}
