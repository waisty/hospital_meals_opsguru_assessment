using System.Net;
using System.Net.Http.Json;
using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;
using Xunit;

namespace Hospital.Meals.WebApi.Tests;

public sealed class MealsAllergyEndpointTests : IClassFixture<MealsWebApiFixture>
{
    private readonly MealsWebApiFixture _fixture;

    public MealsAllergyEndpointTests(MealsWebApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.MockHandler.Clear();
    }

    [Fact]
    public async Task CreateAllergy_WithPatientServiceClaim_Returns201()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientsServiceClaim);
        var response = await client.PostAsJsonAsync("/api/v1/allergies", new AllergyCreateRequest { Id = "peanuts", Name = "Peanuts" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateAllergy_WithMealsUserClaim_Returns403()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.PostAsJsonAsync("/api/v1/allergies", new AllergyCreateRequest { Id = "p", Name = "P" });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAllergy_Existing_Returns204()
    {
        _fixture.MockHandler.SeedAllergy("peanuts", "Peanuts");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientsServiceClaim);

        var response = await client.PutAsJsonAsync("/api/v1/allergies/peanuts", new AllergyUpdateRequest { Name = "Tree Nuts" });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAllergy_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientsServiceClaim);
        var response = await client.PutAsJsonAsync("/api/v1/allergies/missing", new AllergyUpdateRequest { Name = "X" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllergy_Existing_ReturnsOk()
    {
        _fixture.MockHandler.SeedAllergy("shellfish", "Shellfish");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);

        var response = await client.GetAsync("/api/v1/allergies/shellfish");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllergy_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync("/api/v1/allergies/missing");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ListAllergies_ReturnsOk()
    {
        _fixture.MockHandler.SeedAllergy("a1", "A1");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync("/api/v1/allergies");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

public sealed class MealsClinicalStateEndpointTests : IClassFixture<MealsWebApiFixture>
{
    private readonly MealsWebApiFixture _fixture;

    public MealsClinicalStateEndpointTests(MealsWebApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.MockHandler.Clear();
    }

    [Fact]
    public async Task CreateClinicalState_WithPatientServiceClaim_Returns201()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientsServiceClaim);
        var response = await client.PostAsJsonAsync("/api/v1/clinical-states", new ClinicalStateCreateRequest { Id = "diabetic", Name = "Diabetic" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task UpdateClinicalState_Existing_Returns204()
    {
        _fixture.MockHandler.SeedClinicalState("diabetic", "Diabetic");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientsServiceClaim);

        var response = await client.PutAsJsonAsync("/api/v1/clinical-states/diabetic", new ClinicalStateUpdateRequest { Name = "Type 2 Diabetic" });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateClinicalState_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientsServiceClaim);
        var response = await client.PutAsJsonAsync("/api/v1/clinical-states/missing", new ClinicalStateUpdateRequest { Name = "X" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetClinicalState_Existing_ReturnsOk()
    {
        _fixture.MockHandler.SeedClinicalState("hypertension", "Hypertension");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);

        var response = await client.GetAsync("/api/v1/clinical-states/hypertension");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ListClinicalStates_ReturnsOk()
    {
        _fixture.MockHandler.SeedClinicalState("cs1", "CS1");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync("/api/v1/clinical-states");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

public sealed class MealsDietTypeEndpointTests : IClassFixture<MealsWebApiFixture>
{
    private readonly MealsWebApiFixture _fixture;

    public MealsDietTypeEndpointTests(MealsWebApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.MockHandler.Clear();
    }

    [Fact]
    public async Task CreateDietType_WithPatientServiceClaim_Returns201()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientsServiceClaim);
        var response = await client.PostAsJsonAsync("/api/v1/diet-types", new DietTypeCreateRequest { Id = "vegan", Name = "Vegan" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDietType_Existing_Returns204()
    {
        _fixture.MockHandler.SeedDietType("vegan", "Vegan");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientsServiceClaim);

        var response = await client.PutAsJsonAsync("/api/v1/diet-types/vegan", new DietTypeUpdateRequest { Name = "Strict Vegan" });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDietType_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientsServiceClaim);
        var response = await client.PutAsJsonAsync("/api/v1/diet-types/missing", new DietTypeUpdateRequest { Name = "X" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDietType_Existing_ReturnsOk()
    {
        _fixture.MockHandler.SeedDietType("keto", "Keto");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);

        var response = await client.GetAsync("/api/v1/diet-types/keto");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetDietType_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync("/api/v1/diet-types/missing");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ListDietTypes_ReturnsOk()
    {
        _fixture.MockHandler.SeedDietType("dt1", "DT1");
        _fixture.MockHandler.SeedDietType("dt2", "DT2");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);

        var response = await client.GetAsync("/api/v1/diet-types");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
