using System.Net;
using System.Net.Http.Json;
using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;
using Xunit;
using static Hospital.Meals.Core.Contracts.Enums;

namespace Hospital.Meals.WebApi.Tests;

public sealed class PatientRequestEndpointTests : IClassFixture<MealsWebApiFixture>
{
    private readonly MealsWebApiFixture _fixture;

    public PatientRequestEndpointTests(MealsWebApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.MockHandler.Clear();
    }

    [Fact]
    public async Task CreatePatientRequest_Accepted_Returns201()
    {
        _fixture.MockHandler.SetNextPatientRequestOutcome(MealRequestAppprovalStatus.Accepted);

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var request = new PatientRequestCreateRequest { PatientId = Guid.NewGuid().ToString(), RecipeId = "recipe-1" };

        var response = await client.PostAsJsonAsync("/api/v1/patient-requests", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PatientCreateRequestResponse>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrEmpty(body.Id));
        Assert.Equal("Accepted", body.StatusString);
    }

    [Fact]
    public async Task CreatePatientRequest_Rejected_Returns409()
    {
        _fixture.MockHandler.SetNextPatientRequestOutcome(MealRequestAppprovalStatus.Rejected, "Unsafe ingredient detected");

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var request = new PatientRequestCreateRequest { PatientId = Guid.NewGuid().ToString(), RecipeId = "recipe-1" };

        var response = await client.PostAsJsonAsync("/api/v1/patient-requests", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PatientCreateRequestResponse>();
        Assert.NotNull(body);
        Assert.Equal("Rejected", body.StatusString);
    }

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

    [Fact]
    public async Task GetPatientRequest_NotFound_Returns404()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync($"/api/v1/patient-requests/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ListPatientRequests_ReturnsPagedResult()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsUserClaim);
        var response = await client.GetAsync("/api/v1/patient-requests?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PagedResult<PatientRequestViewModel>>();
        Assert.NotNull(body);
    }
}
