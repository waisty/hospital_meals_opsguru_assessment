using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;
using Xunit;

namespace Hospital.Meals.WebApi.Tests;

public sealed class MealEndpointTests : IClassFixture<MealsWebApiFixture>
{
    private readonly MealsWebApiFixture _fixture;

    public MealEndpointTests(MealsWebApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearAll();
    }

    [Fact]
    public async Task Root_ReturnsOk()
    {
        using var client = _fixture.CreateClient();
        var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ── POST /api/v1/meals ──────────────────────────────────────────

    [Fact]
    public async Task CreateMeal_WithMealsAdminClaim_Returns201()
    {
        _fixture.MockRepo.SeedRecipe("recipe-1", "Chicken Soup Recipe");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsAdminClaim);
        var request = new MealCreateRequest { Id = "meal-1", Name = "Chicken Soup" };

        var response = await client.PostAsJsonAsync("/api/v1/meals", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateMeal_WithMealsUserClaim_Returns403()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var request = new MealCreateRequest { Id = "m1", Name = "Test" };

        var response = await client.PostAsJsonAsync("/api/v1/meals", request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateMeal_WithoutAuth_Returns401()
    {
        using var client = _fixture.CreateClient();
        var request = new MealCreateRequest { Id = "m1", Name = "Test" };
        var response = await client.PostAsJsonAsync("/api/v1/meals", request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ── GET /api/v1/meals/{id} ──────────────────────────────────────

    [Fact]
    public async Task GetMeal_Existing_ReturnsOk()
    {
        _fixture.MockRepo.SeedRecipe("recipe-1", "Chicken Soup Recipe");
        _fixture.MockRepo.SeedMeal("meal-1", "Chicken Soup", "recipe-1");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);

        var response = await client.GetAsync("/api/v1/meals/meal-1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<MealViewModel>();
        Assert.NotNull(body);
        Assert.Equal("Chicken Soup", body.Name);
    }

    [Fact]
    public async Task GetMeal_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync("/api/v1/meals/nonexistent");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── GET /api/v1/meals ───────────────────────────────────────────

    [Fact]
    public async Task ListMeals_ReturnsPagedResult()
    {
        _fixture.MockRepo.SeedRecipe("r1", "Recipe 1");
        _fixture.MockRepo.SeedRecipe("r2", "Recipe 2");
        _fixture.MockRepo.SeedMeal("m1", "Meal 1", "r1");
        _fixture.MockRepo.SeedMeal("m2", "Meal 2", "r2");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync("/api/v1/meals?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<MealViewModel>>();
        Assert.NotNull(body);
        Assert.Equal(2, body.TotalCount);
    }

    // ── POST /api/v1/meals/{id}/recipes ────────────────────────────────────

    [Fact]
    public async Task AddRecipeToMeal_RecipeAlreadyInAnotherMeal_Returns409WithExistingMealName()
    {
        _fixture.MockRepo.SeedRecipe("recipe-1", "Chicken Soup");
        _fixture.MockRepo.SeedMeal("meal-a", "Meal A", "recipe-1");
        _fixture.MockRepo.SeedMealOnly("meal-b", "Meal B");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsAdminClaim);
        var request = new AddRecipeToMealRequest { RecipeId = "recipe-1" };
        var response = await client.PostAsJsonAsync("/api/v1/meals/meal-b/recipes", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.TryGetProperty("existingMealName", out var nameProp));
        Assert.Equal("Meal A", nameProp.GetString());
    }
}
