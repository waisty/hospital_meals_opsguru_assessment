using Auth.Core.Contracts;
using Auth.Core.Implementation;
using Auth.UIViewModels;
using WebExceptionHandler;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthServices(builder.Configuration);

// Add services to the container.

var app = builder.Build();



// Configure the HTTP request pipeline.

app.MapPost("/login", async (UserAuthRequest request, IAuthHandler authHandler) =>
{
    var response = await authHandler.AuthenticateUserAsync(request);
    return response is null ? Results.Unauthorized() : Results.Ok(response);
});

//app.MapPost("/get-users", () =>
//{

//}).RequireAuthorization();

//app.MapPost("/get-users", () =>
//{

//}).RequireAuthorization();

app.Run();

