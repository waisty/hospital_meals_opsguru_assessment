using System.Net;
using System.Net.Http.Json;
using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;
using Xunit;

namespace Hospital.Meals.WebApi.Tests;

public sealed class PatientRequestEndpointTests : IClassFixture<MealsWebApiFixture>
{
    private readonly MealsWebApiFixture _fixture;

    public PatientRequestEndpointTests(MealsWebApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearAll();
    }

    private void SeedSafeRecipe()
    {
        _fixture.MockRepo.SeedRecipe("recipe-1", "Chicken Soup");
        _fixture.MockRepo.SeedIngredient("chicken", "Chicken Breast");
        _fixture.MockRepo.SeedIngredient("carrot", "Carrot");
        _fixture.MockRepo.SeedRecipeIngredient("recipe-1", "chicken", 200, "g");
        _fixture.MockRepo.SeedRecipeIngredient("recipe-1", "carrot", 100, "g");
    }

    private void SeedPatientWithNoRestrictions(string patientId)
    {
        _fixture.MockPatientApi.SeedPatient(patientId, "Test", "", "Patient");
    }

    // ── Auth tests ──────────────────────────────────────────────────

    [Fact]
    public async Task CreatePatientRequest_WithoutAuth_Returns401()
    {
        using var client = _fixture.CreateClient();
        var request = new PatientRequestCreateRequest { PatientId = "p1", RecipeId = "r1" };
        var response = await client.PostAsJsonAsync("/api/v1/patient-requests", request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreatePatientRequest_WithMealsAdminOnly_Returns403()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsAdminClaim);
        var request = new PatientRequestCreateRequest { PatientId = "p1", RecipeId = "r1" };
        var response = await client.PostAsJsonAsync("/api/v1/patient-requests", request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // ── Safety check: happy path (accepted) ─────────────────────────

    [Fact]
    public async Task CreatePatientRequest_SafeRecipe_ReturnsAccepted()
    {
        var patientId = Guid.NewGuid().ToString();
        SeedSafeRecipe();
        SeedPatientWithNoRestrictions(patientId);

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var request = new PatientRequestCreateRequest { PatientId = patientId, RecipeId = "recipe-1" };

        var response = await client.PostAsJsonAsync("/api/v1/patient-requests", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PatientCreateRequestResponse>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrEmpty(body.Id));
        Assert.Equal("Accepted", body.StatusString);
        Assert.True(string.IsNullOrEmpty(body.UnsafeIngredientId));
    }

    [Fact]
    public async Task CreatePatientRequest_Accepted_PublishesTrayToKitchen()
    {
        var patientId = Guid.NewGuid().ToString();
        SeedSafeRecipe();
        SeedPatientWithNoRestrictions(patientId);

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var request = new PatientRequestCreateRequest { PatientId = patientId, RecipeId = "recipe-1" };

        await client.PostAsJsonAsync("/api/v1/patient-requests", request);

        Assert.Single(_fixture.MockKitchenApi.PublishedTrays);
        var tray = _fixture.MockKitchenApi.PublishedTrays[0];
        Assert.Equal(patientId, tray.PatientId);
        Assert.Equal("Chicken Soup", tray.RecipeName);
        Assert.Equal(2, tray.Ingredients.Count);
    }

    // ── Safety check: rejected due to allergy ───────────────────────

    [Fact]
    public async Task CreatePatientRequest_AllergyConflict_ReturnsRejected()
    {
        var patientId = Guid.NewGuid().ToString();
        SeedSafeRecipe();
        _fixture.MockRepo.SeedAllergy("poultry-allergy", "Poultry Allergy");
        _fixture.MockRepo.SeedIngredientAllergyExclusion("chicken", "poultry-allergy");
        _fixture.MockPatientApi.SeedPatient(patientId, "Allergic", "", "Patient",
            allergyIds: ["poultry-allergy"]);

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var request = new PatientRequestCreateRequest { PatientId = patientId, RecipeId = "recipe-1" };

        var response = await client.PostAsJsonAsync("/api/v1/patient-requests", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PatientCreateRequestResponse>();
        Assert.NotNull(body);
        Assert.Equal("Rejected", body.StatusString);
        Assert.Equal("Allergen detected", body.StatusReason);
        Assert.Equal("chicken", body.UnsafeIngredientId);
    }

    [Fact]
    public async Task CreatePatientRequest_AllergyOnSecondIngredient_IdentifiesCorrectIngredient()
    {
        var patientId = Guid.NewGuid().ToString();
        SeedSafeRecipe();
        _fixture.MockRepo.SeedAllergy("carrot-allergy", "Carrot Allergy");
        _fixture.MockRepo.SeedIngredientAllergyExclusion("carrot", "carrot-allergy");
        _fixture.MockPatientApi.SeedPatient(patientId, "Carrot", "", "Allergic",
            allergyIds: ["carrot-allergy"]);

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var request = new PatientRequestCreateRequest { PatientId = patientId, RecipeId = "recipe-1" };

        var response = await client.PostAsJsonAsync("/api/v1/patient-requests", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PatientCreateRequestResponse>();
        Assert.NotNull(body);
        Assert.Equal("Rejected", body.StatusString);
        Assert.Equal("carrot", body.UnsafeIngredientId);
    }

    [Fact]
    public async Task CreatePatientRequest_UnrelatedAllergy_ReturnsAccepted()
    {
        var patientId = Guid.NewGuid().ToString();
        SeedSafeRecipe();
        _fixture.MockRepo.SeedAllergy("shellfish-allergy", "Shellfish Allergy");
        _fixture.MockPatientApi.SeedPatient(patientId, "Shellfish", "", "Allergic",
            allergyIds: ["shellfish-allergy"]);

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var request = new PatientRequestCreateRequest { PatientId = patientId, RecipeId = "recipe-1" };

        var response = await client.PostAsJsonAsync("/api/v1/patient-requests", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PatientCreateRequestResponse>();
        Assert.NotNull(body);
        Assert.Equal("Accepted", body.StatusString);
    }

    // ── Safety check: rejected due to clinical state ────────────────

    [Fact]
    public async Task CreatePatientRequest_ClinicalStateConflict_ReturnsRejected()
    {
        var patientId = Guid.NewGuid().ToString();
        SeedSafeRecipe();
        _fixture.MockRepo.SeedClinicalState("renal-failure", "Renal Failure");
        _fixture.MockRepo.SeedIngredientClinicalStateExclusion("chicken", "renal-failure");
        _fixture.MockPatientApi.SeedPatient(patientId, "Renal", "", "Patient",
            clinicalStateIds: ["renal-failure"]);

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var request = new PatientRequestCreateRequest { PatientId = patientId, RecipeId = "recipe-1" };

        var response = await client.PostAsJsonAsync("/api/v1/patient-requests", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PatientCreateRequestResponse>();
        Assert.NotNull(body);
        Assert.Equal("Rejected", body.StatusString);
        Assert.Equal("Clinical state contraindication", body.StatusReason);
        Assert.Equal("chicken", body.UnsafeIngredientId);
    }

    // ── Safety check: rejected due to diet type ─────────────────────

    [Fact]
    public async Task CreatePatientRequest_DietTypeConflict_ReturnsRejected()
    {
        var patientId = Guid.NewGuid().ToString();
        SeedSafeRecipe();
        _fixture.MockRepo.SeedDietType("vegan", "Vegan");
        _fixture.MockRepo.SeedIngredientDietTypeExclusion("chicken", "vegan");
        _fixture.MockPatientApi.SeedPatient(patientId, "Vegan", "", "Patient",
            dietTypeId: "vegan");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var request = new PatientRequestCreateRequest { PatientId = patientId, RecipeId = "recipe-1" };

        var response = await client.PostAsJsonAsync("/api/v1/patient-requests", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PatientCreateRequestResponse>();
        Assert.NotNull(body);
        Assert.Equal("Rejected", body.StatusString);
        Assert.Equal("Diet type exclusion", body.StatusReason);
        Assert.Equal("chicken", body.UnsafeIngredientId);
    }

    [Fact]
    public async Task CreatePatientRequest_DietTypeNoConflict_ReturnsAccepted()
    {
        var patientId = Guid.NewGuid().ToString();
        SeedSafeRecipe();
        _fixture.MockRepo.SeedDietType("keto", "Keto");
        _fixture.MockPatientApi.SeedPatient(patientId, "Keto", "", "Patient",
            dietTypeId: "keto");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var request = new PatientRequestCreateRequest { PatientId = patientId, RecipeId = "recipe-1" };

        var response = await client.PostAsJsonAsync("/api/v1/patient-requests", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PatientCreateRequestResponse>();
        Assert.NotNull(body);
        Assert.Equal("Accepted", body.StatusString);
    }

    // ── GET/List patient requests ───────────────────────────────────

    [Fact]
    public async Task GetPatientRequest_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync($"/api/v1/patient-requests/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPatientRequest_AfterCreate_ReturnsOk()
    {
        var patientId = Guid.NewGuid().ToString();
        SeedSafeRecipe();
        SeedPatientWithNoRestrictions(patientId);

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var createRequest = new PatientRequestCreateRequest { PatientId = patientId, RecipeId = "recipe-1" };
        var createResponse = await client.PostAsJsonAsync("/api/v1/patient-requests", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<PatientCreateRequestResponse>();
        Assert.NotNull(created);

        var getResponse = await client.GetAsync($"/api/v1/patient-requests/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var body = await getResponse.Content.ReadFromJsonAsync<PatientRequestViewModel>();
        Assert.NotNull(body);
        Assert.Equal(patientId, body.PatientId);
        Assert.Equal("recipe-1", body.RecipeId);
        Assert.Equal(MealRequestApprovalStatus.Accepted, body.ApprovalStatus);
    }

    [Fact]
    public async Task ListPatientRequests_ReturnsPagedResult()
    {
        var patientId = Guid.NewGuid().ToString();
        SeedSafeRecipe();
        SeedPatientWithNoRestrictions(patientId);

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        await client.PostAsJsonAsync("/api/v1/patient-requests",
            new PatientRequestCreateRequest { PatientId = patientId, RecipeId = "recipe-1" });

        var response = await client.GetAsync("/api/v1/patient-requests?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<PatientRequestViewModel>>();
        Assert.NotNull(body);
        Assert.True(body.TotalCount >= 1);
    }
}
