using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Hospital.Patient.Core.Contracts;

namespace Hospital.Patient.WebApi.Authentication;

public static class JwtAuthenticationExtensions
{
    /// <summary>Policy name for endpoints that require the patientAdmin claim.</summary>
    public const string PatientAdminPolicyName = "PatientAdmin";
    public const string AdminPolicyName = "Admin";
    public const string MealsServicePolicyName = "MealsService";
    public const string PatientAdminOrMealsServicePolicyName = "PatientAdminOrMealsService";
    public const string PatientAdminOrMealsUserPolicyName = "PatientAdminOrMealsUser";

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
            options.AddPolicy(PatientAdminPolicyName, (policy) =>
            {
                policy.RequireClaim(ClaimIds.patientAdminClaim);
            });
            options.AddPolicy(MealsServicePolicyName, (policy) =>
            {
                policy.RequireClaim(ClaimIds.mealsServiceClaim);
            });
            options.AddPolicy(AdminPolicyName, (policy) =>
            { 
                policy.RequireClaim(ClaimIds.adminClaim); 
            });
            options.AddPolicy(PatientAdminOrMealsServicePolicyName, (policy) =>
            {
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == ClaimIds.patientAdminClaim) || context.User.HasClaim(c => c.Type == ClaimIds.mealsServiceClaim));
            });
            options.AddPolicy(PatientAdminOrMealsUserPolicyName, (policy) =>
            {
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c => c.Type == ClaimIds.patientAdminClaim) || context.User.HasClaim(c => c.Type == ClaimIds.mealsUserClaim));
            });
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
