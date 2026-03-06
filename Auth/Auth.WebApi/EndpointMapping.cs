using Hospital.Auth.Core.Contracts;
using Hospital.Auth.UIViewModels;

namespace Hospital.Auth.WebApi;

public static class EndpointMapping
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new { service = "Hospital.Auth.WebApi", status = "running" }));

        var api = app.MapGroup("/api/v1");

        api.MapPost("/login", async (UserAuthRequest request, IAuthHandler authHandler) =>
        {
            var response = await authHandler.AuthenticateUserAsync(request);
            return response is null ? Results.Unauthorized() : Results.Ok(response);
        });
    }
}
