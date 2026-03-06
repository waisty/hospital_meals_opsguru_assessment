using Hospital.Kitchen.WebApi;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapEndpoints();

app.Run();

/// <summary>Entry point type for integration tests (WebApplicationFactory).</summary>
public partial class Program { }
