using System.Net;
using System.Net.Http.Json;
using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;
using Xunit;

namespace Hospital.Meals.WebApi.Tests;

public sealed class IngredientEndpointTests : IClassFixture<MealsWebApiFixture>
{
    private readonly MealsWebApiFixture _fixture;

    public IngredientEndpointTests(MealsWebApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearAll();
    }

    [Fact]
    public async Task CreateIngredient_WithMealsAdminClaim_Returns201()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsAdminClaim);
        var request = new IngredientCreateRequest { Id = "chicken", Name = "Chicken Breast", Description = "Lean protein" };

        var response = await client.PostAsJsonAsync("/api/v1/ingredients", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateIngredient_WithMealsUserClaim_Returns403()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var request = new IngredientCreateRequest { Id = "i1", Name = "Test" };
        var response = await client.PostAsJsonAsync("/api/v1/ingredients", request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetIngredient_Existing_ReturnsOk()
    {
        _fixture.MockRepo.SeedIngredient("chicken", "Chicken Breast");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);

        var response = await client.GetAsync("/api/v1/ingredients/chicken");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<IngredientViewModel>();
        Assert.NotNull(body);
        Assert.Equal("Chicken Breast", body.Name);
    }

    [Fact]
    public async Task GetIngredient_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync("/api/v1/ingredients/missing");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetIngredientDetail_Existing_ReturnsOk()
    {
        _fixture.MockRepo.SeedIngredient("chicken", "Chicken");
        _fixture.MockRepo.SeedIngredientAllergyExclusion("chicken", "poultry-allergy");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);

        var response = await client.GetAsync("/api/v1/ingredients/chicken/detail");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<IngredientDetailViewModel>();
        Assert.NotNull(body);
        Assert.Equal("Chicken", body.Name);
        Assert.Single(body.AllergyExclusionIds);
        Assert.Equal("poultry-allergy", body.AllergyExclusionIds[0]);
    }

    [Fact]
    public async Task GetIngredientDetail_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync("/api/v1/ingredients/missing/detail");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ListIngredients_ReturnsPagedResult()
    {
        _fixture.MockRepo.SeedIngredient("i1", "I1");
        _fixture.MockRepo.SeedIngredient("i2", "I2");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync("/api/v1/ingredients?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<IngredientViewModel>>();
        Assert.NotNull(body);
        Assert.Equal(2, body.TotalCount);
    }

    // ── Ingredient exclusions ───────────────────────────────────────

    [Fact]
    public async Task GetIngredientAllergyExclusions_ReturnsOk()
    {
        _fixture.MockRepo.SeedIngredient("chicken", "Chicken");
        _fixture.MockRepo.SeedIngredientAllergyExclusion("chicken", "peanuts");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync("/api/v1/ingredients/chicken/allergies");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<List<string>>();
        Assert.NotNull(body);
        Assert.Single(body);
    }

    [Fact]
    public async Task SetIngredientAllergyExclusions_WithMealsAdminClaim_Returns204()
    {
        _fixture.MockRepo.SeedIngredient("chicken", "Chicken");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsAdminClaim);
        var request = new SetIngredientAllergyExclusionsRequest { AllergyIds = ["peanuts", "shellfish"] };

        var response = await client.PutAsJsonAsync("/api/v1/ingredients/chicken/allergies", request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetIngredientClinicalStateExclusions_ReturnsOk()
    {
        _fixture.MockRepo.SeedIngredient("chicken", "Chicken");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync("/api/v1/ingredients/chicken/clinical-states");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SetIngredientClinicalStateExclusions_WithMealsAdminClaim_Returns204()
    {
        _fixture.MockRepo.SeedIngredient("chicken", "Chicken");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsAdminClaim);
        var request = new SetIngredientClinicalStateExclusionsRequest { ClinicalStateIds = ["diabetic"] };

        var response = await client.PutAsJsonAsync("/api/v1/ingredients/chicken/clinical-states", request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetIngredientDietTypeExclusions_ReturnsOk()
    {
        _fixture.MockRepo.SeedIngredient("chicken", "Chicken");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync("/api/v1/ingredients/chicken/diet-types");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SetIngredientDietTypeExclusions_WithMealsUserClaim_Returns403()
    {
        _fixture.MockRepo.SeedIngredient("chicken", "Chicken");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var request = new SetIngredientDietTypeExclusionsRequest { DietTypeIds = ["vegan"] };

        var response = await client.PutAsJsonAsync("/api/v1/ingredients/chicken/diet-types", request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task SetIngredientDietTypeExclusions_WithMealsAdminClaim_Returns204_AndPersists()
    {
        _fixture.MockRepo.SeedIngredient("chicken", "Chicken");

        using (var adminClient = _fixture.CreateAuthenticatedClient(ClaimIds.mealsAdminClaim))
        {
            var request = new SetIngredientDietTypeExclusionsRequest { DietTypeIds = ["vegan", "vegetarian"] };
            var putResponse = await adminClient.PutAsJsonAsync("/api/v1/ingredients/chicken/diet-types", request);
            Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);
        }

        using (var userClient = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim))
        {
            var getResponse = await userClient.GetAsync("/api/v1/ingredients/chicken/diet-types");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var body = await getResponse.Content.ReadFromJsonAsync<List<string>>();
            Assert.NotNull(body);
            Assert.Equal(2, body.Count);
            Assert.Contains("vegan", body);
            Assert.Contains("vegetarian", body);
        }
    }
}
