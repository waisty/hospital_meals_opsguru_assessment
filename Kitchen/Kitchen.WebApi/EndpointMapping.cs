using Hospital.Kitchen.Core.Contracts;
using Hospital.Kitchen.WebApi.Authentication;

namespace Hospital.Kitchen.WebApi;

public static class EndpointMapping
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new { service = "Hospital.Kitchen.WebApi", status = "running" }));

        var api = app.MapGroup("/api/v1");

        api.MapPost("/trays", async (CreateTrayRequest request, IKitchenHandler handler, CancellationToken ct) =>
        {
            var trayId = await handler.CreateTrayAsync(request, ct);
            return Results.Created($"/api/v1/trays/{trayId}", new { id = trayId });
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsServicePolicyName);
    }
}
