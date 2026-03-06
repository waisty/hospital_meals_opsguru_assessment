using Hospital.Kitchen.Core;
using Hospital.Kitchen.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKitchenServices(builder.Configuration);

var app = builder.Build();

app.MapEndpoints();

app.Run();

/// <summary>Entry point type for integration tests (WebApplicationFactory).</summary>
public partial class Program { }
