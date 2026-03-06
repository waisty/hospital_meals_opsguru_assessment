using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Hospital.Meals.Core.Contracts;

namespace Hospital.Meals.WebApi.Authentication;

public static class JwtAuthenticationExtensions
{
    public const string MealsAdminPolicyName = "MealsAdmin";
    public const string MealsUserPolicyName = "MealsUser";
    public const string PatientServicePolicyName = "PatientService";

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured.");
        var jwtIssuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer not configured.");
        var jwtAudience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience not configured.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(MealsAdminPolicyName, policy => policy.RequireClaim(ClaimIds.mealsAdminClaim));
            options.AddPolicy(PatientServicePolicyName, policy => policy.RequireClaim(ClaimIds.patientsServiceClaim));
            options.AddPolicy(MealsUserPolicyName, policy => policy.RequireClaim(ClaimIds.mealsUserClaim));
        });

        return services;
    }

    public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}
