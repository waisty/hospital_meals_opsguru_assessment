using Auth.Core.Implementation;
using WebExceptionHandler;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthServices(builder.Configuration);

// Add services to the container.

var app = builder.Build();



// Configure the HTTP request pipeline.

app.MapPost("/login", () =>
{

});

//app.MapPost("/get-users", () =>
//{

//}).RequireAuthorization();

//app.MapPost("/get-users", () =>
//{

//}).RequireAuthorization();

app.Run();

