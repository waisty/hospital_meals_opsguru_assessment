using WebExceptionHandler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Add services to the container.

var app = builder.Build();



// Configure the HTTP request pipeline.

app.MapPost("/login", () =>
{

});

//app.MapPost("/get-users", () =>
//{

//}).RequireAuthorization();

app.MapPost("/get-users", () =>
{

}).RequireAuthorization();

app.Run();

