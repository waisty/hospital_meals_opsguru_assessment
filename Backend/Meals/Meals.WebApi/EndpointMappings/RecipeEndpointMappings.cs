using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;
using Hospital.Meals.WebApi.Authentication;
using Microsoft.AspNetCore.Routing;

namespace Hospital.Meals.WebApi.EndpointMappings;

public static class RecipeEndpointMappings
{
    public static RouteGroupBuilder MapRecipeEndpointMappings(this RouteGroupBuilder group)
    {
        group.MapPost("/recipes", async (RecipeCreateRequest request, IRecipeHandler handler, CancellationToken ct) =>
        {
            await handler.AddRecipeAsync(request, ct);
            return Results.Created($"/api/v1/recipes/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        group.MapGet("/recipes/{id}", async (string id, IRecipeHandler handler, CancellationToken ct) =>
        {
            var recipe = await handler.GetRecipeByIdAsync(id, ct);
            return recipe is null ? Results.NotFound() : Results.Ok(recipe);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapGet("/recipes/{id}/detail", async (string id, IRecipeHandler handler, CancellationToken ct) =>
        {
            var detail = await handler.GetRecipeDetailByIdAsync(id, ct);
            return detail is null ? Results.NotFound() : Results.Ok(detail);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapGet("/recipes", async (int page, int pageSize, IRecipeHandler handler, CancellationToken ct) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            var result = await handler.ListRecipesAsync(page, pageSize, ct);
            return Results.Ok(result);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapGet("/recipes/{recipeId}/ingredients", async (string recipeId, IRecipeHandler handler, CancellationToken ct) =>
        {
            var list = await handler.GetRecipeIngredientsByRecipeIdAsync(recipeId, ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapPut("/recipes/{recipeId}/ingredients", async (string recipeId, SetRecipeIngredientsRequest request, IRecipeHandler handler, CancellationToken ct) =>
        {
            await handler.SetRecipeIngredientsAsync(recipeId, request, ct);
            return Results.NoContent();
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        return group;
    }
}
