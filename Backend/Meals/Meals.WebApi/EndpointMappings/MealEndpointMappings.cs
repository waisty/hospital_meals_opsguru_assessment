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

        group.MapGet("/meals", async (int page, int pageSize, string? search, IMealHandler handler, CancellationToken ct) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            var result = await handler.ListMealsAsync(page, pageSize, search, ct);
            return Results.Ok(result);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapPut("/meals/{id}", async (string id, MealUpdateRequest request, IMealHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdateMealAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        group.MapGet("/meals/{id}/recipes", async (string id, IMealHandler handler, CancellationToken ct) =>
        {
            var recipes = await handler.GetMealRecipesAsync(id, ct);
            return Results.Ok(recipes);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapPost("/meals/{id}/recipes", async (string id, AddRecipeToMealRequest request, IMealHandler handler, CancellationToken ct) =>
        {
            var added = await handler.AddRecipeToMealAsync(id, request.RecipeId, ct);
            return added ? Results.NoContent() : Results.Conflict();
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        group.MapPut("/meals/{mealId}/recipes/{recipeId}/disabled", async (string mealId, string recipeId, SetMealRecipeDisabledRequest request, IMealHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.SetMealRecipeDisabledAsync(mealId, recipeId, request.Disabled, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        return group;
    }
}
