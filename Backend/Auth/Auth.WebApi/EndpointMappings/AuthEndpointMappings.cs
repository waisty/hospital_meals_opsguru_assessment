using Hospital.Auth.Core.Contracts;
using Hospital.Auth.ViewModels;
using Microsoft.AspNetCore.Routing;

namespace Hospital.Auth.WebApi.EndpointMappings;

public static class AuthEndpointMappings
{
    public static RouteGroupBuilder MapAuthEndpointMappings(this RouteGroupBuilder group)
    {
        group.MapPost("/login", async (UserAuthRequest request, IAuthHandler authHandler) =>
        {
            var response = await authHandler.AuthenticateUserAsync(request);
            return response is null ? Results.Unauthorized() : Results.Ok(response);
        });

        return group;
    }
}
