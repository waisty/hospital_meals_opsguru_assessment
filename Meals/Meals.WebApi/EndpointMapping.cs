using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;
using Hospital.Meals.WebApi.Authentication;
using Hospital.Meals.WebApi.Validation;

namespace Hospital.Meals.WebApi;

public static class EndpointMapping
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new { service = "Hospital.Meals.WebApi", status = "running" }));

        var api = app.MapGroup("/api/v1").AddEndpointFilter<ValidationEndpointFilter>();

        // Meal
        api.MapPost("/meals", async (MealCreateRequest request, IMealsHandler handler, CancellationToken ct) =>
        {
            await handler.AddMealAsync(request, ct);
            return Results.Created($"/api/v1/meals/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/meals/{id}", async (string id, IMealsHandler handler, CancellationToken ct) =>
        {
            var meal = await handler.GetMealByIdAsync(id, ct);
            return meal is null ? Results.NotFound() : Results.Ok(meal);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/meals", async (int page, int pageSize, IMealsHandler handler, CancellationToken ct) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            var result = await handler.ListMealsAsync(page, pageSize, ct);
            return Results.Ok(result);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        // Recipe
        api.MapPost("/recipes", async (RecipeCreateRequest request, IMealsHandler handler, CancellationToken ct) =>
        {
            await handler.AddRecipeAsync(request, ct);
            return Results.Created($"/api/v1/recipes/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/recipes/{id}", async (string id, IMealsHandler handler, CancellationToken ct) =>
        {
            var recipe = await handler.GetRecipeByIdAsync(id, ct);
            return recipe is null ? Results.NotFound() : Results.Ok(recipe);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/recipes/{id}/detail", async (string id, IMealsHandler handler, CancellationToken ct) =>
        {
            var detail = await handler.GetRecipeDetailByIdAsync(id, ct);
            return detail is null ? Results.NotFound() : Results.Ok(detail);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/recipes", async (int page, int pageSize, IMealsHandler handler, CancellationToken ct) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            var result = await handler.ListRecipesAsync(page, pageSize, ct);
            return Results.Ok(result);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        // Recipe ingredients
        api.MapGet("/recipes/{recipeId}/ingredients", async (string recipeId, IMealsHandler handler, CancellationToken ct) =>
        {
            var list = await handler.GetRecipeIngredientsByRecipeIdAsync(recipeId, ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapPut("/recipes/{recipeId}/ingredients", async (string recipeId, SetRecipeIngredientsRequest request, IMealsHandler handler, CancellationToken ct) =>
        {
            await handler.SetRecipeIngredientsAsync(recipeId, request, ct);
            return Results.NoContent();
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        // Ingredient
        api.MapPost("/ingredients", async (IngredientCreateRequest request, IMealsHandler handler, CancellationToken ct) =>
        {
            await handler.AddIngredientAsync(request, ct);
            return Results.Created($"/api/v1/ingredients/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/ingredients/{id}", async (string id, IMealsHandler handler, CancellationToken ct) =>
        {
            var ingredient = await handler.GetIngredientByIdAsync(id, ct);
            return ingredient is null ? Results.NotFound() : Results.Ok(ingredient);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/ingredients/{id}/detail", async (string id, IMealsHandler handler, CancellationToken ct) =>
        {
            var detail = await handler.GetIngredientDetailByIdAsync(id, ct);
            return detail is null ? Results.NotFound() : Results.Ok(detail);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/ingredients", async (int page, int pageSize, IMealsHandler handler, CancellationToken ct) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            var result = await handler.ListIngredientsAsync(page, pageSize, ct);
            return Results.Ok(result);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        // Ingredient exclusions
        api.MapGet("/ingredients/{ingredientId}/allergies", async (string ingredientId, IMealsHandler handler, CancellationToken ct) =>
        {
            var ids = await handler.GetAllergyIdsByIngredientIdAsync(ingredientId, ct);
            return Results.Ok(ids);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapPut("/ingredients/{ingredientId}/allergies", async (string ingredientId, SetIngredientAllergyExclusionsRequest request, IMealsHandler handler, CancellationToken ct) =>
        {
            await handler.SetAllergyExclusionsForIngredientAsync(ingredientId, request, ct);
            return Results.NoContent();
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/ingredients/{ingredientId}/clinical-states", async (string ingredientId, IMealsHandler handler, CancellationToken ct) =>
        {
            var ids = await handler.GetClinicalStateIdsByIngredientIdAsync(ingredientId, ct);
            return Results.Ok(ids);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapPut("/ingredients/{ingredientId}/clinical-states", async (string ingredientId, SetIngredientClinicalStateExclusionsRequest request, IMealsHandler handler, CancellationToken ct) =>
        {
            await handler.SetClinicalStateExclusionsForIngredientAsync(ingredientId, request, ct);
            return Results.NoContent();
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/ingredients/{ingredientId}/diet-types", async (string ingredientId, IMealsHandler handler, CancellationToken ct) =>
        {
            var ids = await handler.GetDietTypeExclusionIdsByIngredientIdAsync(ingredientId, ct);
            return Results.Ok(ids);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapPut("/ingredients/{ingredientId}/diet-types", async (string ingredientId, SetIngredientDietTypeExclusionsRequest request, IMealsHandler handler, CancellationToken ct) =>
        {
            await handler.SetDietTypeExclusionsForIngredientAsync(ingredientId, request, ct);
            return Results.NoContent();
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        // Patient meal request
        api.MapPost("/meal-requests", async (PatientMealRequestCreateRequest request, IMealsHandler handler, CancellationToken ct) =>
        {
            var id = await handler.AddPatientMealRequestAsync(request, ct);
            return Results.Created($"/api/v1/meal-requests/{id}", new { Id = id.ToString() });
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/meal-requests/{id}", async (string id, IMealsHandler handler, CancellationToken ct) =>
        {
            var request = await handler.GetPatientMealRequestByIdAsync(id, ct);
            return request is null ? Results.NotFound() : Results.Ok(request);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/meal-requests", async (int page, int pageSize, IMealsHandler handler, CancellationToken ct) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            var result = await handler.ListPatientMealRequestsAsync(page, pageSize, ct);
            return Results.Ok(result);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        // Allergy (reference data)
        api.MapPost("/allergies", async (AllergyCreateRequest request, IMealsHandler handler, CancellationToken ct) =>
        {
            await handler.AddAllergyAsync(request, ct);
            return Results.Created($"/api/v1/allergies/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        api.MapPut("/allergies/{id}", async (string id, AllergyUpdateRequest request, IMealsHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdateAllergyAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        api.MapGet("/allergies/{id}", async (string id, IMealsHandler handler, CancellationToken ct) =>
        {
            var allergy = await handler.GetAllergyByIdAsync(id, ct);
            return allergy is null ? Results.NotFound() : Results.Ok(allergy);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/allergies", async (IMealsHandler handler, CancellationToken ct) =>
        {
            var list = await handler.ListAllergiesAsync(ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        // Clinical state (reference data)
        api.MapPost("/clinical-states", async (ClinicalStateCreateRequest request, IMealsHandler handler, CancellationToken ct) =>
        {
            await handler.AddClinicalStateAsync(request, ct);
            return Results.Created($"/api/v1/clinical-states/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        api.MapPut("/clinical-states/{id}", async (string id, ClinicalStateUpdateRequest request, IMealsHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdateClinicalStateAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        api.MapGet("/clinical-states/{id}", async (string id, IMealsHandler handler, CancellationToken ct) =>
        {
            var clinicalState = await handler.GetClinicalStateByIdAsync(id, ct);
            return clinicalState is null ? Results.NotFound() : Results.Ok(clinicalState);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/clinical-states", async (IMealsHandler handler, CancellationToken ct) =>
        {
            var list = await handler.ListClinicalStatesAsync(ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        // Diet type (reference data)
        api.MapPost("/diet-types", async (DietTypeCreateRequest request, IMealsHandler handler, CancellationToken ct) =>
        {
            await handler.AddDietTypeAsync(request, ct);
            return Results.Created($"/api/v1/diet-types/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        api.MapPut("/diet-types/{id}", async (string id, DietTypeUpdateRequest request, IMealsHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdateDietTypeAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        api.MapGet("/diet-types/{id}", async (string id, IMealsHandler handler, CancellationToken ct) =>
        {
            var dietType = await handler.GetDietTypeByIdAsync(id, ct);
            return dietType is null ? Results.NotFound() : Results.Ok(dietType);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/diet-types", async (IMealsHandler handler, CancellationToken ct) =>
        {
            var list = await handler.ListDietTypesAsync(ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);
    }
}
