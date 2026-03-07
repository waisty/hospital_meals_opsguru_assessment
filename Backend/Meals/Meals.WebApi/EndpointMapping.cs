using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;
using Hospital.Meals.WebApi.Authentication;
using Hospital.Meals.WebApi.Validation;
using static Hospital.Meals.Core.Contracts.Enums;

namespace Hospital.Meals.WebApi;

public static class EndpointMapping
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new { service = "Hospital.Meals.WebApi", status = "running" }));

        var api = app.MapGroup("/api/v1").AddEndpointFilter<ValidationEndpointFilter>();

        // Meal
        api.MapPost("/meals", async (MealCreateRequest request, IMealHandler handler, CancellationToken ct) =>
        {
            await handler.AddMealAsync(request, ct);
            return Results.Created($"/api/v1/meals/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/meals/{id}", async (string id, IMealHandler handler, CancellationToken ct) =>
        {
            var meal = await handler.GetMealByIdAsync(id, ct);
            return meal is null ? Results.NotFound() : Results.Ok(meal);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        api.MapGet("/meals", async (int page, int pageSize, IMealHandler handler, CancellationToken ct) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            var result = await handler.ListMealsAsync(page, pageSize, ct);
            return Results.Ok(result);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        // Recipe
        api.MapPost("/recipes", async (RecipeCreateRequest request, IRecipeHandler handler, CancellationToken ct) =>
        {
            await handler.AddRecipeAsync(request, ct);
            return Results.Created($"/api/v1/recipes/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/recipes/{id}", async (string id, IRecipeHandler handler, CancellationToken ct) =>
        {
            var recipe = await handler.GetRecipeByIdAsync(id, ct);
            return recipe is null ? Results.NotFound() : Results.Ok(recipe);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        api.MapGet("/recipes/{id}/detail", async (string id, IRecipeHandler handler, CancellationToken ct) =>
        {
            var detail = await handler.GetRecipeDetailByIdAsync(id, ct);
            return detail is null ? Results.NotFound() : Results.Ok(detail);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        api.MapGet("/recipes", async (int page, int pageSize, IRecipeHandler handler, CancellationToken ct) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            var result = await handler.ListRecipesAsync(page, pageSize, ct);
            return Results.Ok(result);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        // Recipe ingredients
        api.MapGet("/recipes/{recipeId}/ingredients", async (string recipeId, IRecipeHandler handler, CancellationToken ct) =>
        {
            var list = await handler.GetRecipeIngredientsByRecipeIdAsync(recipeId, ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        api.MapPut("/recipes/{recipeId}/ingredients", async (string recipeId, SetRecipeIngredientsRequest request, IRecipeHandler handler, CancellationToken ct) =>
        {
            await handler.SetRecipeIngredientsAsync(recipeId, request, ct);
            return Results.NoContent();
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        // Ingredient
        api.MapPost("/ingredients", async (IngredientCreateRequest request, IIngredientHandler handler, CancellationToken ct) =>
        {
            await handler.AddIngredientAsync(request, ct);
            return Results.Created($"/api/v1/ingredients/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/ingredients/{id}", async (string id, IIngredientHandler handler, CancellationToken ct) =>
        {
            var ingredient = await handler.GetIngredientByIdAsync(id, ct);
            return ingredient is null ? Results.NotFound() : Results.Ok(ingredient);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        api.MapGet("/ingredients/{id}/detail", async (string id, IIngredientHandler handler, CancellationToken ct) =>
        {
            var detail = await handler.GetIngredientDetailByIdAsync(id, ct);
            return detail is null ? Results.NotFound() : Results.Ok(detail);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        api.MapGet("/ingredients", async (int page, int pageSize, IIngredientHandler handler, CancellationToken ct) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            var result = await handler.ListIngredientsAsync(page, pageSize, ct);
            return Results.Ok(result);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        // Ingredient exclusions
        api.MapGet("/ingredients/{ingredientId}/allergies", async (string ingredientId, IIngredientHandler handler, CancellationToken ct) =>
        {
            var ids = await handler.GetAllergyIdsByIngredientIdAsync(ingredientId, ct);
            return Results.Ok(ids);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        api.MapPut("/ingredients/{ingredientId}/allergies", async (string ingredientId, SetIngredientAllergyExclusionsRequest request, IIngredientHandler handler, CancellationToken ct) =>
        {
            await handler.SetAllergyExclusionsForIngredientAsync(ingredientId, request, ct);
            return Results.NoContent();
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/ingredients/{ingredientId}/clinical-states", async (string ingredientId, IIngredientHandler handler, CancellationToken ct) =>
        {
            var ids = await handler.GetClinicalStateIdsByIngredientIdAsync(ingredientId, ct);
            return Results.Ok(ids);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        api.MapPut("/ingredients/{ingredientId}/clinical-states", async (string ingredientId, SetIngredientClinicalStateExclusionsRequest request, IIngredientHandler handler, CancellationToken ct) =>
        {
            await handler.SetClinicalStateExclusionsForIngredientAsync(ingredientId, request, ct);
            return Results.NoContent();
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

        api.MapGet("/ingredients/{ingredientId}/diet-types", async (string ingredientId, IIngredientHandler handler, CancellationToken ct) =>
        {
            var ids = await handler.GetDietTypeExclusionIdsByIngredientIdAsync(ingredientId, ct);
            return Results.Ok(ids);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        api.MapPut("/ingredients/{ingredientId}/diet-types", async (string ingredientId, SetIngredientDietTypeExclusionsRequest request, IIngredientHandler handler, CancellationToken ct) =>
        {
            await handler.SetDietTypeExclusionsForIngredientAsync(ingredientId, request, ct);
            return Results.NoContent();
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        // Patient request
        api.MapPost("/patient-requests", async (PatientRequestCreateRequest request, IPatientRequestHandler handler, CancellationToken ct) =>
        {
            var requestInfo = await handler.AddPatientRequestAsync(request, ct);
            var response = new PatientCreateRequestResponse()
            {
                Id = requestInfo.requestId.ToString(),
                StatusReason = requestInfo.statusReason ?? "",
                StatusString = requestInfo.status.ToString(),
                UnsafeIngredientId = requestInfo.unsafeIngredientId
            };

            return requestInfo.status == MealRequestAppprovalStatus.Rejected ? Results.Conflict(response) : Results.Created($"/api/v1/patient-requests/{response.Id}", response);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        api.MapGet("/patient-requests/{id}", async (string id, IPatientRequestHandler handler, CancellationToken ct) =>
        {
            var request = await handler.GetPatientRequestByIdAsync(id, ct);
            return request is null ? Results.NotFound() : Results.Ok(request);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        api.MapGet("/patient-requests", async (int page, int pageSize, IPatientRequestHandler handler, CancellationToken ct) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            var result = await handler.ListPatientRequestsAsync(page, pageSize, ct);
            return Results.Ok(result);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        // Allergy (reference data)
        api.MapPost("/allergies", async (AllergyCreateRequest request, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            await handler.AddAllergyAsync(request, ct);
            return Results.Created($"/api/v1/allergies/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        api.MapPut("/allergies/{id}", async (string id, AllergyUpdateRequest request, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdateAllergyAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        api.MapGet("/allergies/{id}", async (string id, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var allergy = await handler.GetAllergyByIdAsync(id, ct);
            return allergy is null ? Results.NotFound() : Results.Ok(allergy);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        api.MapGet("/allergies", async (IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var list = await handler.ListAllergiesAsync(ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        // Clinical state (reference data)
        api.MapPost("/clinical-states", async (ClinicalStateCreateRequest request, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            await handler.AddClinicalStateAsync(request, ct);
            return Results.Created($"/api/v1/clinical-states/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        api.MapPut("/clinical-states/{id}", async (string id, ClinicalStateUpdateRequest request, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdateClinicalStateAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        api.MapGet("/clinical-states/{id}", async (string id, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var clinicalState = await handler.GetClinicalStateByIdAsync(id, ct);
            return clinicalState is null ? Results.NotFound() : Results.Ok(clinicalState);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        api.MapGet("/clinical-states", async (IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var list = await handler.ListClinicalStatesAsync(ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        // Diet type (reference data)
        api.MapPost("/diet-types", async (DietTypeCreateRequest request, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            await handler.AddDietTypeAsync(request, ct);
            return Results.Created($"/api/v1/diet-types/{request.Id}", null);
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        api.MapPut("/diet-types/{id}", async (string id, DietTypeUpdateRequest request, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var updated = await handler.UpdateDietTypeAsync(id, request, ct);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

        api.MapGet("/diet-types/{id}", async (string id, IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var dietType = await handler.GetDietTypeByIdAsync(id, ct);
            return dietType is null ? Results.NotFound() : Results.Ok(dietType);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);

        api.MapGet("/diet-types", async (IReferenceDataHandler handler, CancellationToken ct) =>
        {
            var list = await handler.ListDietTypesAsync(ct);
            return Results.Ok(list);
        }).RequireAuthorization(JwtAuthenticationExtensions.MealsUserPolicyName);
    }
}
