using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;
using Hospital.Meals.WebApi.Authentication;
using Microsoft.AspNetCore.Routing;

namespace Hospital.Meals.WebApi.EndpointMappings;

public static class IngredientEndpointMappings
{
    public static RouteGroupBuilder MapIngredientEndpointMappings(this RouteGroupBuilder group)
    {
        group.MapPost("/ingredients", async (IngredientCreateRequest request, IIngredientHandler handler, CancellationToken ct) =>
        {
            await handler.AddIngredientAsync(request, ct);
            return Results.Created($"/api/v1/ingredients/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        group.MapGet("/ingredients/{id}", async (string id, IIngredientHandler handler, CancellationToken ct) =>
        {
            var ingredient = await handler.GetIngredientByIdAsync(id, ct);
            return ingredient is null ? Results.NotFound() : Results.Ok(ingredient);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapGet("/ingredients/{id}/detail", async (string id, IIngredientHandler handler, CancellationToken ct) =>
        {
            var detail = await handler.GetIngredientDetailByIdAsync(id, ct);
            return detail is null ? Results.NotFound() : Results.Ok(detail);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapGet("/ingredients", async (int page, int pageSize, IIngredientHandler handler, CancellationToken ct) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            var result = await handler.ListIngredientsAsync(page, pageSize, ct);
            return Results.Ok(result);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapGet("/ingredients/{ingredientId}/allergies", async (string ingredientId, IIngredientHandler handler, CancellationToken ct) =>
        {
            var ids = await handler.GetAllergyIdsByIngredientIdAsync(ingredientId, ct);
            return Results.Ok(ids);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapPut("/ingredients/{ingredientId}/allergies", async (string ingredientId, SetIngredientAllergyExclusionsRequest request, IIngredientHandler handler, CancellationToken ct) =>
        {
            await handler.SetAllergyExclusionsForIngredientAsync(ingredientId, request, ct);
            return Results.NoContent();
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        group.MapGet("/ingredients/{ingredientId}/clinical-states", async (string ingredientId, IIngredientHandler handler, CancellationToken ct) =>
        {
            var ids = await handler.GetClinicalStateIdsByIngredientIdAsync(ingredientId, ct);
            return Results.Ok(ids);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapPut("/ingredients/{ingredientId}/clinical-states", async (string ingredientId, SetIngredientClinicalStateExclusionsRequest request, IIngredientHandler handler, CancellationToken ct) =>
        {
            await handler.SetClinicalStateExclusionsForIngredientAsync(ingredientId, request, ct);
            return Results.NoContent();
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        group.MapGet("/ingredients/{ingredientId}/diet-types", async (string ingredientId, IIngredientHandler handler, CancellationToken ct) =>
        {
            var ids = await handler.GetDietTypeExclusionIdsByIngredientIdAsync(ingredientId, ct);
            return Results.Ok(ids);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        group.MapPut("/ingredients/{ingredientId}/diet-types", async (string ingredientId, SetIngredientDietTypeExclusionsRequest request, IIngredientHandler handler, CancellationToken ct) =>
        {
            await handler.SetDietTypeExclusionsForIngredientAsync(ingredientId, request, ct);
            return Results.NoContent();
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        return group;
    }
}
