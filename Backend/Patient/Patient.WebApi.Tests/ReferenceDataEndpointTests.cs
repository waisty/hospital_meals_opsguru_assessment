using System.Net;
using System.Net.Http.Json;
using Hospital.Patient.Core.Contracts;
using Hospital.Patient.ViewModels;
using Xunit;

namespace Hospital.Patient.WebApi.Tests;

public sealed class AllergyEndpointTests : IClassFixture<PatientWebApiFixture>
{
    private readonly PatientWebApiFixture _fixture;

    public AllergyEndpointTests(PatientWebApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearAll();
    }

    [Fact]
    public async Task CreateAllergy_WithAdminClaim_Returns201()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.adminClaim);
        var response = await client.PostAsJsonAsync("/api/v1/allergies", new AllergyCreateRequest { Name = "Peanuts" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AllergyCreateResponse>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrEmpty(body.Id));
    }

    [Fact]
    public async Task CreateAllergy_WithPatientAdminClaim_Returns403()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var response = await client.PostAsJsonAsync("/api/v1/allergies", new AllergyCreateRequest { Name = "Peanuts" });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAllergy_Existing_Returns204()
    {
        _fixture.MockHandler.SeedAllergy("peanuts", "Peanuts");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.adminClaim);

        var response = await client.PutAsJsonAsync("/api/v1/allergies/peanuts", new AllergyUpdateRequest { Name = "Peanuts (Tree Nut)" });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAllergy_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.adminClaim);
        var response = await client.PutAsJsonAsync("/api/v1/allergies/nonexistent", new AllergyUpdateRequest { Name = "X" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllergy_Existing_ReturnsOk()
    {
        _fixture.MockHandler.SeedAllergy("shellfish", "Shellfish");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);

        var response = await client.GetAsync("/api/v1/allergies/shellfish");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<AllergyViewModel>();
        Assert.NotNull(body);
        Assert.Equal("Shellfish", body.Name);
    }

    [Fact]
    public async Task GetAllergy_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var response = await client.GetAsync("/api/v1/allergies/missing");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ListAllergies_ReturnsOk()
    {
        _fixture.MockHandler.SeedAllergy("a1", "A1");
        _fixture.MockHandler.SeedAllergy("a2", "A2");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var response = await client.GetAsync("/api/v1/allergies");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<List<AllergyViewModel>>();
        Assert.NotNull(list);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetPatientAllergies_ReturnsOk()
    {
        var patientId = Guid.NewGuid();
        _fixture.MockHandler.SeedPatient(patientId, "P", "", "1", "r");
        await _fixture.MockHandler.UpdatePatientAllergiesAsync(patientId.ToString(), new PatientAllergiesUpdateRequest { AllergyIds = ["a1", "a2"] });

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var response = await client.GetAsync($"/api/v1/patients/{patientId}/allergies");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var ids = await response.Content.ReadFromJsonAsync<List<string>>();
        Assert.NotNull(ids);
        Assert.Equal(2, ids.Count);
    }

    [Fact]
    public async Task UpdatePatientAllergies_Existing_Returns204()
    {
        var patientId = Guid.NewGuid();
        _fixture.MockHandler.SeedPatient(patientId, "P", "", "1", "r");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var response = await client.PutAsJsonAsync($"/api/v1/patients/{patientId}/allergies",
            new PatientAllergiesUpdateRequest { AllergyIds = ["a1"] });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePatientAllergies_PatientNotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var response = await client.PutAsJsonAsync($"/api/v1/patients/{Guid.NewGuid()}/allergies",
            new PatientAllergiesUpdateRequest { AllergyIds = ["a1"] });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

public sealed class ClinicalStateEndpointTests : IClassFixture<PatientWebApiFixture>
{
    private readonly PatientWebApiFixture _fixture;

    public ClinicalStateEndpointTests(PatientWebApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearAll();
    }

    [Fact]
    public async Task CreateClinicalState_WithAdminClaim_Returns201()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.adminClaim);
        var response = await client.PostAsJsonAsync("/api/v1/clinical-states", new ClinicalStateCreateRequest { Name = "Diabetic" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ClinicalStateCreateResponse>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrEmpty(body.Id));
    }

    [Fact]
    public async Task UpdateClinicalState_Existing_Returns204()
    {
        _fixture.MockHandler.SeedClinicalState("diabetic", "Diabetic");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.adminClaim);

        var response = await client.PutAsJsonAsync("/api/v1/clinical-states/diabetic", new ClinicalStateUpdateRequest { Name = "Type 2 Diabetic" });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateClinicalState_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.adminClaim);
        var response = await client.PutAsJsonAsync("/api/v1/clinical-states/missing", new ClinicalStateUpdateRequest { Name = "X" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetClinicalState_Existing_ReturnsOk()
    {
        _fixture.MockHandler.SeedClinicalState("hypertension", "Hypertension");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);

        var response = await client.GetAsync("/api/v1/clinical-states/hypertension");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ListClinicalStates_ReturnsOk()
    {
        _fixture.MockHandler.SeedClinicalState("cs1", "CS1");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);

        var response = await client.GetAsync("/api/v1/clinical-states");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePatientClinicalStates_Existing_Returns204()
    {
        var patientId = Guid.NewGuid();
        _fixture.MockHandler.SeedPatient(patientId, "P", "", "1", "r");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var response = await client.PutAsJsonAsync($"/api/v1/patients/{patientId}/clinical-states",
            new PatientClinicalStatesUpdateRequest { ClinicalStateIds = ["cs1"] });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePatientClinicalStates_PatientNotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var response = await client.PutAsJsonAsync($"/api/v1/patients/{Guid.NewGuid()}/clinical-states",
            new PatientClinicalStatesUpdateRequest { ClinicalStateIds = ["cs1"] });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

public sealed class DietTypeEndpointTests : IClassFixture<PatientWebApiFixture>
{
    private readonly PatientWebApiFixture _fixture;

    public DietTypeEndpointTests(PatientWebApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearAll();
    }

    [Fact]
    public async Task CreateDietType_WithAdminClaim_Returns201()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.adminClaim);
        var response = await client.PostAsJsonAsync("/api/v1/diet-types", new DietTypeCreateRequest { Name = "Vegan" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<DietTypeCreateResponse>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrEmpty(body.Id));
    }

    [Fact]
    public async Task CreateDietType_WithPatientAdminClaim_Returns403()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var response = await client.PostAsJsonAsync("/api/v1/diet-types", new DietTypeCreateRequest { Name = "Vegan" });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDietType_Existing_Returns204()
    {
        _fixture.MockHandler.SeedDietType("vegan", "Vegan");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.adminClaim);

        var response = await client.PutAsJsonAsync("/api/v1/diet-types/vegan", new DietTypeUpdateRequest { Name = "Strict Vegan" });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDietType_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.adminClaim);
        var response = await client.PutAsJsonAsync("/api/v1/diet-types/missing", new DietTypeUpdateRequest { Name = "X" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDietType_Existing_ReturnsOk()
    {
        _fixture.MockHandler.SeedDietType("keto", "Keto");
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);

        var response = await client.GetAsync("/api/v1/diet-types/keto");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<DietTypeViewModel>();
        Assert.NotNull(body);
        Assert.Equal("Keto", body.Name);
    }

    [Fact]
    public async Task GetDietType_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var response = await client.GetAsync("/api/v1/diet-types/missing");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ListDietTypes_ReturnsOk()
    {
        _fixture.MockHandler.SeedDietType("dt1", "DT1");
        _fixture.MockHandler.SeedDietType("dt2", "DT2");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var response = await client.GetAsync("/api/v1/diet-types");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<List<DietTypeViewModel>>();
        Assert.NotNull(list);
        Assert.Equal(2, list.Count);
    }
}
