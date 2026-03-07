using System.Net;
using System.Net.Http.Json;
using Hospital.Patient.Core.Contracts;
using Hospital.Patient.ViewModels;
using Xunit;

namespace Hospital.Patient.WebApi.Tests;

public sealed class PatientEndpointTests : IClassFixture<PatientWebApiFixture>
{
    private readonly PatientWebApiFixture _fixture;

    public PatientEndpointTests(PatientWebApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearAll();
    }

    // ── Health ───────────────────────────────────────────────────────

    [Fact]
    public async Task Root_ReturnsOk()
    {
        using var client = _fixture.CreateClient();
        var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ── POST /api/v1/patients ───────────────────────────────────────

    [Fact]
    public async Task CreatePatient_WithAdminClaim_Returns201()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var request = new PatientCreateRequest { FirstName = "Jane", MiddleName = "", LastName = "Doe", MobileNumber = "555-1234", DietTypeId = "regular" };

        var response = await client.PostAsJsonAsync("/api/v1/patients", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PatientCreateResponse>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrEmpty(body.Id));
    }

    [Fact]
    public async Task CreatePatient_WithoutAuth_Returns401()
    {
        using var client = _fixture.CreateClient();
        var request = new PatientCreateRequest { FirstName = "Jane", MiddleName = "", LastName = "Doe", MobileNumber = "555-1234" };
        var response = await client.PostAsJsonAsync("/api/v1/patients", request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreatePatient_WithWrongClaim_Returns403()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsServiceClaim);
        var request = new PatientCreateRequest { FirstName = "Jane", MiddleName = "", LastName = "", MobileNumber = "555-1234" };
        var response = await client.PostAsJsonAsync("/api/v1/patients", request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // ── GET /api/v1/patients/{id} ───────────────────────────────────

    [Fact]
    public async Task GetPatient_Existing_ReturnsOk()
    {
        var id = Guid.NewGuid();
        _fixture.MockHandler.SeedPatient(id, "John", "", "", "555-0000", "regular");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var response = await client.GetAsync($"/api/v1/patients/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PatientViewModel>();
        Assert.NotNull(body);
        Assert.Equal("John", body.FirstName);
        Assert.Equal("", body.LastName);
    }

    [Fact]
    public async Task GetPatient_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var response = await client.GetAsync($"/api/v1/patients/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPatient_InvalidGuid_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var response = await client.GetAsync("/api/v1/patients/not-a-guid");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── GET /api/v1/patients/{id}/detail ────────────────────────────

    [Fact]
    public async Task GetPatientDetail_WithPatientAdminClaim_ReturnsOk()
    {
        var id = Guid.NewGuid();
        _fixture.MockHandler.SeedPatient(id, "Detail", "", "Patient", "555-9999", "vegan");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var response = await client.GetAsync($"/api/v1/patients/{id}/detail");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PatientDetailViewModel>();
        Assert.NotNull(body);
        Assert.Equal("Detail", body.FirstName);
        Assert.Equal("Patient", body.LastName);
    }

    [Fact]
    public async Task GetPatientServiceDetail_WithMealsServiceClaim_ReturnsOk()
    {
        var id = Guid.NewGuid();
        _fixture.MockHandler.SeedPatient(id, "Service", "", "Patient", "555-8888", "regular");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsServiceClaim);
        var response = await client.GetAsync($"/api/v1/patients/{id}/service-detail");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetPatientDetail_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var response = await client.GetAsync($"/api/v1/patients/{Guid.NewGuid()}/detail");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ── GET /api/v1/patients?page&pageSize ──────────────────────────

    [Fact]
    public async Task ListPatients_ReturnsPagedResult()
    {
        _fixture.MockHandler.SeedPatient(Guid.NewGuid(), "A", "", "", "1", "r");
        _fixture.MockHandler.SeedPatient(Guid.NewGuid(), "B", "", "", "2", "r");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.patientAdminClaim);
        var response = await client.GetAsync("/api/v1/patients?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<PatientViewModel>>();
        Assert.NotNull(body);
        Assert.Equal(2, body.TotalCount);
    }
}
