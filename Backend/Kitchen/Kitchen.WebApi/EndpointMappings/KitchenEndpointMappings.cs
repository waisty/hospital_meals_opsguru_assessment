using Enums = Hospital.Kitchen.Core.Contracts.Enums;
using Hospital.Kitchen.Core.Contracts;
using Hospital.Kitchen.ServiceViewModels;
using Hospital.Kitchen.ViewModels;
using Hospital.Kitchen.WebApi.Authentication;
using Microsoft.AspNetCore.Routing;

namespace Hospital.Kitchen.WebApi.EndpointMappings;

public static class KitchenEndpointMappings
{
    public static RouteGroupBuilder MapKitchenEndpointMappings(this RouteGroupBuilder group)
    {
        group.MapGet("/trays", async (int? page, int? pageSize, int? state, bool? uncompletedOnly, IKitchenHandler handler, CancellationToken ct) =>
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

        group.MapPost("/trays", async (CreateTrayRequest request, IKitchenHandler handler, CancellationToken ct) =>
        {
            var trayId = await handler.CreateTrayAsync(request, ct);
            return Results.Created($"/api/v1/trays/{trayId}", new { id = trayId });
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsServicePolicyName);

        group.MapPost("/trays/advance-state", async (AdvanceTrayStateRequest request, IKitchenHandler handler, CancellationToken ct) =>
        {
            var success = await handler.AdvanceTrayStateAsync(request.TrayId, (Enums.TrayState)request.FromState, ct);
            return success
                ? Results.Ok(new AdvanceTrayStateResponse { Success = true })
                : Results.Conflict(new AdvanceTrayStateResponse { Success = false });
        }).RequireAuthorization(JwtAuthenticationExtensions.KitchenUserPolicyName);

        return group;
    }
}
