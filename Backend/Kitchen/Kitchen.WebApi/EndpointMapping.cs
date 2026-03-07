using Hospital.Kitchen.Core.Contracts;
using Hospital.Kitchen.ServiceViewModels;
using Hospital.Kitchen.ViewModels;
using Hospital.Kitchen.WebApi.Authentication;
using Enums = Hospital.Kitchen.Core.Contracts.Enums;

namespace Hospital.Kitchen.WebApi;

public static class EndpointMapping
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new { service = "Hospital.Kitchen.WebApi", status = "running" }));

        var api = app.MapGroup("/api/v1");

        api.MapGet("/trays", async (int? page, int? pageSize, int? state, bool? uncompletedOnly, IKitchenHandler handler, CancellationToken ct) =>
        {
            var p = page is null or < 1 ? 1 : page.Value;
            var ps = pageSize is null or < 1 ? 10 : Math.Min(100, pageSize.Value);
            var stateFilter = state.HasValue && Enum.IsDefined(typeof(Enums.TrayState), state.Value)
                ? (Enums.TrayState?)state.Value
                : null;
            var uncompleted = uncompletedOnly ?? false;
            var result = await handler.ListTraysAsync(p, ps, stateFilter, uncompleted, ct);
            return Results.Ok(result);
        }).RequireAuthorization(JwtAuthenticationExtensions.KitchenUserPolicyName);

        api.MapPost("/trays", async (CreateTrayRequest request, IKitchenHandler handler, CancellationToken ct) =>
        {
            var trayId = await handler.CreateTrayAsync(request, ct);
            return Results.Created($"/api/v1/trays/{trayId}", new { id = trayId });
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsServicePolicyName);

        api.MapPost("/trays/advance-state", async (AdvanceTrayStateRequest request, IKitchenHandler handler, CancellationToken ct) =>
        {
            var success = await handler.AdvanceTrayStateAsync(request.TrayId, (Enums.TrayState)request.FromState, ct);
            return success
                ? Results.Ok(new AdvanceTrayStateResponse { Success = true })
                : Results.Conflict(new AdvanceTrayStateResponse { Success = false });
        }).RequireAuthorization(JwtAuthenticationExtensions.KitchenUserPolicyName);
    }
}
