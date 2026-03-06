using Hospital.Patient.Core;
using Hospital.Patient.WebApi;
using Hospital.Patient.WebApi.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddPatientServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseJwtAuthentication();

app.MapEndpoints();

app.Run();

/// <summary>Entry point type for integration tests (WebApplicationFactory).</summary>
public partial class Program { }
