using System.Net;
using System.Net.Http.Json;
using Hospital.Kitchen.Core.Contracts;
using Hospital.Kitchen.ServiceViewModels;
using Hospital.Kitchen.ViewModels;
using Xunit;
using CoreCreateTrayRequest = Hospital.Kitchen.ServiceViewModels.CreateTrayRequest;
using CoreTrayIngredientItem = Hospital.Kitchen.ServiceViewModels.TrayIngredientItem;

namespace Hospital.Kitchen.WebApi.Tests;

public sealed class TrayEndpointTests : IClassFixture<KitchenWebApiFixture>
{
    private readonly KitchenWebApiFixture _fixture;

    public TrayEndpointTests(KitchenWebApiFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearAll();
    }

    // ── Health / root ───────────────────────────────────────────────

    [Fact]
    public async Task Root_ReturnsOk()
    {
        using var client = _fixture.CreateClient();
        var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ── POST /api/v1/trays ──────────────────────────────────────────

    [Fact]
    public async Task CreateTray_WithMealsServiceClaim_Returns201()
    {
        var trayId = Guid.NewGuid();
        _fixture.MockHandler.SetNextTrayId(trayId);

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsServiceClaim);
        var request = new CoreCreateTrayRequest
        {
            PatientMealRequestId = Guid.NewGuid(),
            PatientId = "patient-1",
            PatientName = "John Doe",
            RecipeName = "Chicken Soup",
            State = Enums.TrayState.Pending,
            Ingredients =
            [
                new CoreTrayIngredientItem { IngredientName = "Chicken", Qty = 200, Unit = "g" },
                new CoreTrayIngredientItem { IngredientName = "Noodles", Qty = 100, Unit = "g" }
            ]
        };

        var response = await client.PostAsJsonAsync("/api/v1/trays", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Contains($"/api/v1/trays/{trayId}", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task CreateTray_WithoutAuth_Returns401()
    {
        using var client = _fixture.CreateClient();
        var request = new CoreCreateTrayRequest
        {
            PatientMealRequestId = Guid.NewGuid(),
            PatientId = "p1",
            PatientName = "Test",
            RecipeName = "Test Recipe"
        };

        var response = await client.PostAsJsonAsync("/api/v1/trays", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateTray_WithWrongClaim_Returns403()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.kitchenUserClaim);
        var request = new CoreCreateTrayRequest
        {
            PatientMealRequestId = Guid.NewGuid(),
            PatientId = "p1",
            PatientName = "Test",
            RecipeName = "Test Recipe"
        };

        var response = await client.PostAsJsonAsync("/api/v1/trays", request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateTray_WithEmptyIngredients_Returns201()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsServiceClaim);
        var request = new CoreCreateTrayRequest
        {
            PatientMealRequestId = Guid.NewGuid(),
            PatientId = "p1",
            PatientName = "Test",
            RecipeName = "Test Recipe",
            Ingredients = []
        };

        var response = await client.PostAsJsonAsync("/api/v1/trays", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    // ── POST /api/v1/trays/advance-state ────────────────────────────

    [Fact]
    public async Task AdvanceTrayState_ValidTransition_ReturnsOk()
    {
        var trayId = Guid.NewGuid();
        _fixture.MockHandler.AddTray(trayId, Enums.TrayState.Pending);

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.kitchenUserClaim);
        var request = new AdvanceTrayStateRequest { TrayId = trayId, FromState = (int)Enums.TrayState.Pending };

        var response = await client.PostAsJsonAsync("/api/v1/trays/advance-state", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AdvanceTrayStateResponse>();
        Assert.NotNull(body);
        Assert.True(body.Success);
    }

    [Fact]
    public async Task AdvanceTrayState_WrongFromState_ReturnsConflict()
    {
        var trayId = Guid.NewGuid();
        _fixture.MockHandler.AddTray(trayId, Enums.TrayState.PreparationStarted);

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.kitchenUserClaim);
        var request = new AdvanceTrayStateRequest { TrayId = trayId, FromState = (int)Enums.TrayState.Pending };

        var response = await client.PostAsJsonAsync("/api/v1/trays/advance-state", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AdvanceTrayStateResponse>();
        Assert.NotNull(body);
        Assert.False(body.Success);
    }

    [Fact]
    public async Task AdvanceTrayState_TrayNotFound_ReturnsConflict()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.kitchenUserClaim);
        var request = new AdvanceTrayStateRequest { TrayId = Guid.NewGuid(), FromState = (int)Enums.TrayState.Pending };

        var response = await client.PostAsJsonAsync("/api/v1/trays/advance-state", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task AdvanceTrayState_AlreadyRetrieved_ReturnsConflict()
    {
        var trayId = Guid.NewGuid();
        _fixture.MockHandler.AddTray(trayId, Enums.TrayState.Retrieved);

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.kitchenUserClaim);
        var request = new AdvanceTrayStateRequest { TrayId = trayId, FromState = (int)Enums.TrayState.Retrieved };

        var response = await client.PostAsJsonAsync("/api/v1/trays/advance-state", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task AdvanceTrayState_WithoutAuth_Returns401()
    {
        using var client = _fixture.CreateClient();
        var request = new AdvanceTrayStateRequest { TrayId = Guid.NewGuid(), FromState = 0 };

        var response = await client.PostAsJsonAsync("/api/v1/trays/advance-state", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AdvanceTrayState_WithMealsServiceClaim_Returns403()
    {
        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.mealsServiceClaim);
        var request = new AdvanceTrayStateRequest { TrayId = Guid.NewGuid(), FromState = 0 };

        var response = await client.PostAsJsonAsync("/api/v1/trays/advance-state", request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdvanceTrayState_FullLifecycle_Succeeds()
    {
        var trayId = Guid.NewGuid();
        _fixture.MockHandler.AddTray(trayId, Enums.TrayState.Pending);

        using var client = _fixture.CreateAuthenticatedClient(ClaimIds.kitchenUserClaim);

        var states = new[]
        {
            Enums.TrayState.Pending,
            Enums.TrayState.PreparationStarted,
            Enums.TrayState.AccuracyValidated,
            Enums.TrayState.EnRoute,
            Enums.TrayState.Delivered
        };

        foreach (var state in states)
        {
            var request = new AdvanceTrayStateRequest { TrayId = trayId, FromState = (int)state };
            var response = await client.PostAsJsonAsync("/api/v1/trays/advance-state", request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
