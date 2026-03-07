using Hospital.Kitchen.WebApi.EndpointMappings;

namespace Hospital.Kitchen.WebApi;

public static class EndpointMapping
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new { service = "Hospital.Kitchen.WebApi", status = "running" }));

        var api = app.MapGroup("/api/v1");

        api.MapKitchenEndpointMappings();
    }
}
