using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;
using Hospital.Meals.WebApi.Authentication;
using Microsoft.AspNetCore.Routing;

namespace Hospital.Meals.WebApi.EndpointMappings;

public static class MealEndpointMappings
{
    public static RouteGroupBuilder MapMealEndpointMappings(this RouteGroupBuilder group)
    {
        group.MapPost("/meals", async (MealCreateRequest request, IMealHandler handler, CancellationToken ct) =>
        {
            await handler.AddMealAsync(request, ct);
            return Results.Created($"/api/v1/meals/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        group.MapGet("/meals/{id}", async (string id, IMealHandler handler, CancellationToken ct) =>
        {
            var meal = await handler.GetMealByIdAsync(id, ct);
            return meal is null ? Results.NotFound() : Results.Ok(meal);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapGet("/meals", async (int page, int pageSize, IMealHandler handler, CancellationToken ct) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            var result = await handler.ListMealsAsync(page, pageSize, ct);
            return Results.Ok(result);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        return group;
    }
}
