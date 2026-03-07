using System.Net;
using System.Net.Http.Json;
using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;
using Xunit;

namespace Hospital.Meals.WebApi.Tests;

public sealed class RecipeEndpointTests : IClassFixture<MealsWebApiFixture>
{
    private readonly MealsWebApiFixture _fixture;

    public RecipeEndpointTests(MealsWebApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.MockHandler.Clear();
    }

    [Fact]
    public async Task CreateRecipe_WithMealsAdminClaim_Returns201()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsAdminClaim);
        var request = new RecipeCreateRequest { Id = "recipe-1", Name = "Chicken Soup Recipe", Description = "A classic" };

        var response = await client.PostAsJsonAsync("/api/v1/recipes", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateRecipe_WithMealsUserClaim_Returns403()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var request = new RecipeCreateRequest { Id = "r1", Name = "Test" };

        var response = await client.PostAsJsonAsync("/api/v1/recipes", request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipe_Existing_ReturnsOk()
    {
        _fixture.MockHandler.SeedRecipe("recipe-1", "Chicken Soup Recipe");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);

        var response = await client.GetAsync("/api/v1/recipes/recipe-1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<RecipeViewModel>();
        Assert.NotNull(body);
        Assert.Equal("Chicken Soup Recipe", body.Name);
    }

    [Fact]
    public async Task GetRecipe_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync("/api/v1/recipes/nonexistent");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipeDetail_Existing_ReturnsOk()
    {
        _fixture.MockHandler.SeedRecipe("recipe-det", "Detail Recipe");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);

        var response = await client.GetAsync("/api/v1/recipes/recipe-det/detail");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<RecipeDetailViewModel>();
        Assert.NotNull(body);
        Assert.Equal("Detail Recipe", body.Name);
    }

    [Fact]
    public async Task GetRecipeDetail_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync("/api/v1/recipes/missing/detail");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ListRecipes_ReturnsPagedResult()
    {
        _fixture.MockHandler.SeedRecipe("r1", "R1");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);

        var response = await client.GetAsync("/api/v1/recipes?page=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ── Recipe ingredients ──────────────────────────────────────────

    [Fact]
    public async Task GetRecipeIngredients_ReturnsOk()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync("/api/v1/recipes/r1/ingredients");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SetRecipeIngredients_WithMealsAdminClaim_Returns204()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsAdminClaim);
        var request = new SetRecipeIngredientsRequest
        {
            Ingredients = [new RecipeIngredientViewModel { IngredientId = "chicken", Quantity = 200, Unit = "g" }]
        };

        var response = await client.PutAsJsonAsync("/api/v1/recipes/r1/ingredients", request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task SetRecipeIngredients_WithMealsUserClaim_Returns403()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var request = new SetRecipeIngredientsRequest { Ingredients = [] };

        var response = await client.PutAsJsonAsync("/api/v1/recipes/r1/ingredients", request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
