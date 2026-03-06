using Hospital.Meals.Core;
using Hospital.Meals.Core.Contracts;
using Hospital.Meals.UIViewModels;
using Hospital.Meals.WebApi;
using Hospital.Meals.WebApi.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddMealsServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { service = "Hospital.Meals.WebApi", status = "running" }));

app.UseJwtAuthentication();

// Meal
app.MapPost("/meals", async (MealCreateRequest request, IMealsHandler handler, CancellationToken ct) =>
{
    await handler.AddMealAsync(request, ct);
    return Results.Created($"/meals/{request.Id}", null);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapGet("/meals/{id}", async (string id, IMealsHandler handler, CancellationToken ct) =>
{
    var meal = await handler.GetMealByIdAsync(id, ct);
    return meal is null ? Results.NotFound() : Results.Ok(meal);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapGet("/meals", async (int page, int pageSize, IMealsHandler handler, CancellationToken ct) =>
{
    if (page < 1) page = 1;
    if (pageSize < 1 || pageSize > 100) pageSize = 10;
    var result = await handler.ListMealsAsync(page, pageSize, ct);
    return Results.Ok(result);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

// Recipe
app.MapPost("/recipes", async (RecipeCreateRequest request, IMealsHandler handler, CancellationToken ct) =>
{
    await handler.AddRecipeAsync(request, ct);
    return Results.Created($"/recipes/{request.Id}", null);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapGet("/recipes/{id}", async (string id, IMealsHandler handler, CancellationToken ct) =>
{
    var recipe = await handler.GetRecipeByIdAsync(id, ct);
    return recipe is null ? Results.NotFound() : Results.Ok(recipe);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapGet("/recipes/{id}/detail", async (string id, IMealsHandler handler, CancellationToken ct) =>
{
    var detail = await handler.GetRecipeDetailByIdAsync(id, ct);
    return detail is null ? Results.NotFound() : Results.Ok(detail);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapGet("/recipes", async (int page, int pageSize, IMealsHandler handler, CancellationToken ct) =>
{
    if (page < 1) page = 1;
    if (pageSize < 1 || pageSize > 100) pageSize = 10;
    var result = await handler.ListRecipesAsync(page, pageSize, ct);
    return Results.Ok(result);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

// Recipe ingredients
app.MapGet("/recipes/{recipeId}/ingredients", async (string recipeId, IMealsHandler handler, CancellationToken ct) =>
{
    var list = await handler.GetRecipeIngredientsByRecipeIdAsync(recipeId, ct);
    return Results.Ok(list);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapPut("/recipes/{recipeId}/ingredients", async (string recipeId, SetRecipeIngredientsRequest request, IMealsHandler handler, CancellationToken ct) =>
{
    await handler.SetRecipeIngredientsAsync(recipeId, request, ct);
    return Results.NoContent();
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

// Ingredient
app.MapPost("/ingredients", async (IngredientCreateRequest request, IMealsHandler handler, CancellationToken ct) =>
{
    await handler.AddIngredientAsync(request, ct);
    return Results.Created($"/ingredients/{request.Id}", null);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapGet("/ingredients/{id}", async (string id, IMealsHandler handler, CancellationToken ct) =>
{
    var ingredient = await handler.GetIngredientByIdAsync(id, ct);
    return ingredient is null ? Results.NotFound() : Results.Ok(ingredient);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapGet("/ingredients/{id}/detail", async (string id, IMealsHandler handler, CancellationToken ct) =>
{
    var detail = await handler.GetIngredientDetailByIdAsync(id, ct);
    return detail is null ? Results.NotFound() : Results.Ok(detail);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapGet("/ingredients", async (int page, int pageSize, IMealsHandler handler, CancellationToken ct) =>
{
    if (page < 1) page = 1;
    if (pageSize < 1 || pageSize > 100) pageSize = 10;
    var result = await handler.ListIngredientsAsync(page, pageSize, ct);
    return Results.Ok(result);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

// Ingredient exclusions
app.MapGet("/ingredients/{ingredientId}/allergies", async (string ingredientId, IMealsHandler handler, CancellationToken ct) =>
{
    var ids = await handler.GetAllergyIdsByIngredientIdAsync(ingredientId, ct);
    return Results.Ok(ids);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapPut("/ingredients/{ingredientId}/allergies", async (string ingredientId, SetIngredientAllergyExclusionsRequest request, IMealsHandler handler, CancellationToken ct) =>
{
    await handler.SetAllergyExclusionsForIngredientAsync(ingredientId, request, ct);
    return Results.NoContent();
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapGet("/ingredients/{ingredientId}/clinical-states", async (string ingredientId, IMealsHandler handler, CancellationToken ct) =>
{
    var ids = await handler.GetClinicalStateIdsByIngredientIdAsync(ingredientId, ct);
    return Results.Ok(ids);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapPut("/ingredients/{ingredientId}/clinical-states", async (string ingredientId, SetIngredientClinicalStateExclusionsRequest request, IMealsHandler handler, CancellationToken ct) =>
{
    await handler.SetClinicalStateExclusionsForIngredientAsync(ingredientId, request, ct);
    return Results.NoContent();
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapGet("/ingredients/{ingredientId}/diet-types", async (string ingredientId, IMealsHandler handler, CancellationToken ct) =>
{
    var ids = await handler.GetDietTypeExclusionIdsByIngredientIdAsync(ingredientId, ct);
    return Results.Ok(ids);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapPut("/ingredients/{ingredientId}/diet-types", async (string ingredientId, SetIngredientDietTypeExclusionsRequest request, IMealsHandler handler, CancellationToken ct) =>
{
    await handler.SetDietTypeExclusionsForIngredientAsync(ingredientId, request, ct);
    return Results.NoContent();
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

// Patient meal request
app.MapPost("/meal-requests", async (PatientMealRequestCreateRequest request, IMealsHandler handler, CancellationToken ct) =>
{
    var id = await handler.AddPatientMealRequestAsync(request, ct);
    return Results.Created($"/meal-requests/{id}", new { Id = id.ToString() });
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapGet("/meal-requests/{id}", async (string id, IMealsHandler handler, CancellationToken ct) =>
{
    var request = await handler.GetPatientMealRequestByIdAsync(id, ct);
    return request is null ? Results.NotFound() : Results.Ok(request);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapGet("/meal-requests", async (int page, int pageSize, IMealsHandler handler, CancellationToken ct) =>
{
    if (page < 1) page = 1;
    if (pageSize < 1 || pageSize > 100) pageSize = 10;
    var result = await handler.ListPatientMealRequestsAsync(page, pageSize, ct);
    return Results.Ok(result);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

// Allergy (reference data)
app.MapPost("/allergies", async (AllergyCreateRequest request, IMealsHandler handler, CancellationToken ct) =>
{
    await handler.AddAllergyAsync(request, ct);
    return Results.Created($"/allergies/{request.Id}", null);
}).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

app.MapGet("/allergies/{id}", async (string id, IMealsHandler handler, CancellationToken ct) =>
{
    var allergy = await handler.GetAllergyByIdAsync(id, ct);
    return allergy is null ? Results.NotFound() : Results.Ok(allergy);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapGet("/allergies", async (IMealsHandler handler, CancellationToken ct) =>
{
    var list = await handler.ListAllergiesAsync(ct);
    return Results.Ok(list);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

// Clinical state (reference data)
app.MapPost("/clinical-states", async (ClinicalStateCreateRequest request, IMealsHandler handler, CancellationToken ct) =>
{
    await handler.AddClinicalStateAsync(request, ct);
    return Results.Created($"/clinical-states/{request.Id}", null);
}).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

app.MapGet("/clinical-states/{id}", async (string id, IMealsHandler handler, CancellationToken ct) =>
{
    var clinicalState = await handler.GetClinicalStateByIdAsync(id, ct);
    return clinicalState is null ? Results.NotFound() : Results.Ok(clinicalState);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapGet("/clinical-states", async (IMealsHandler handler, CancellationToken ct) =>
{
    var list = await handler.ListClinicalStatesAsync(ct);
    return Results.Ok(list);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

// Diet type (reference data)
app.MapPost("/diet-types", async (DietTypeCreateRequest request, IMealsHandler handler, CancellationToken ct) =>
{
    await handler.AddDietTypeAsync(request, ct);
    return Results.Created($"/diet-types/{request.Id}", null);
}).RequireAuthorization(JwtAuthenticationExtensions.PatientServicePolicyName);

app.MapGet("/diet-types/{id}", async (string id, IMealsHandler handler, CancellationToken ct) =>
{
    var dietType = await handler.GetDietTypeByIdAsync(id, ct);
    return dietType is null ? Results.NotFound() : Results.Ok(dietType);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.MapGet("/diet-types", async (IMealsHandler handler, CancellationToken ct) =>
{
    var list = await handler.ListDietTypesAsync(ct);
    return Results.Ok(list);
}).RequireAuthorization(JwtAuthenticationExtensions.MealsAdminPolicyName);

app.Run();

/// <summary>Entry point type for integration tests (WebApplicationFactory).</summary>
public partial class Program { }
