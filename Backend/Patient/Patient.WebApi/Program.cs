using Hospital.Patient.Core;
using Hospital.Patient.WebApi;
using Hospital.Patient.WebApi.Authentication;
using Hospital.Patient.WebApi.EndpointMappings;
using Hospital.Patient.WebApi.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? [];
if (allowedOrigins.Length > 0)
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
        });
    });
}

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddPatientServices(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
}

var app = builder.Build();

if (allowedOrigins.Length > 0)
    app.UseCors();
app.UseExceptionHandler();
app.UseJwtAuthentication();

app.MapGet("/", () => Results.Ok(new { service = "Hospital.Patient.WebApi", status = "running" }));
var api = app.MapGroup("/api/v1").AddEndpointFilter<ValidationEndpointFilter>();
api.MapPatientEndpointMappings();
api.MapAllergyEndpointMappings();
api.MapClinicalStateEndpointMappings();
api.MapDietTypeEndpointMappings();

app.Run();

/// <summary>Entry point type for integration tests (WebApplicationFactory).</summary>
public partial class Program { }
