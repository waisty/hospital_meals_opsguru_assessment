using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using Hospital.Auth.ViewModels;
using Xunit;

namespace Hospital.Auth.WebApi.Tests;

public sealed class LoginEndpointTests : IClassFixture<AuthWebApiFixture>
{
    private readonly AuthWebApiFixture _fixture;

    public LoginEndpointTests(AuthWebApiFixture fixture)
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

    // ── Valid credentials ───────────────────────────────────────────

    [Fact]
    public async Task Login_WithValidCredentials_Returns200_AndToken()
    {
        _fixture.MockAuthRepo.AddUser("testuser", "testpass", name: "Test User", admin: true);

        using var client = _fixture.CreateClient();
        var request = new UserAuthRequest { Username = "testuser", Password = "testpass" };
        var response = await client.PostAsJsonAsync("/api/v1/login", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<UserAuthResponse>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrEmpty(body.AuthToken));
    }

    // ── Invalid credentials ─────────────────────────────────────────

    [Fact]
    public async Task Login_WithInvalidCredentials_Returns401()
    {
        _fixture.MockAuthRepo.AddUser("validuser", "validpass");

        using var client = _fixture.CreateClient();
        var request = new UserAuthRequest { Username = "validuser", Password = "wrongpass" };
        var response = await client.PostAsJsonAsync("/api/v1/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithUnknownUser_Returns401()
    {
        using var client = _fixture.CreateClient();
        var request = new UserAuthRequest { Username = "nobody", Password = "any" };
        var response = await client.PostAsJsonAsync("/api/v1/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ── Admin role flags in response ────────────────────────────────

    [Fact]
    public async Task Login_AdminUser_ResponseReflectsAdminFlags()
    {
        _fixture.MockAuthRepo.AddUser("admin", "adminpass", name: "Admin", admin: true);

        using var client = _fixture.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/login", new UserAuthRequest { Username = "admin", Password = "adminpass" });

        var body = await response.Content.ReadFromJsonAsync<UserAuthResponse>();
        Assert.NotNull(body);
        Assert.True(body.Admin);
        Assert.True(body.PatientAdmin);
        Assert.True(body.MealsAdmin);
        Assert.True(body.MealsUser);
        Assert.True(body.KitchenUser);
    }

    [Fact]
    public async Task Login_MealsOnlyUser_ResponseReflectsRoleFlags()
    {
        _fixture.MockAuthRepo.AddUser("mealsuser", "pass", name: "Meals User", mealsUser: true);

        using var client = _fixture.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/login", new UserAuthRequest { Username = "mealsuser", Password = "pass" });

        var body = await response.Content.ReadFromJsonAsync<UserAuthResponse>();
        Assert.NotNull(body);
        Assert.False(body.Admin);
        Assert.False(body.PatientAdmin);
        Assert.False(body.MealsAdmin);
        Assert.True(body.MealsUser);
        Assert.False(body.KitchenUser);
    }

    [Fact]
    public async Task Login_MealsAdminUser_ResponseReflectsRoleFlags()
    {
        _fixture.MockAuthRepo.AddUser("mealsadmin", "pass", name: "Meals Admin", mealsAdmin: true);

        using var client = _fixture.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/login", new UserAuthRequest { Username = "mealsadmin", Password = "pass" });

        var body = await response.Content.ReadFromJsonAsync<UserAuthResponse>();
        Assert.NotNull(body);
        Assert.True(body.MealsAdmin);
        Assert.False(body.Admin);
    }

    [Fact]
    public async Task Login_KitchenUser_ResponseReflectsRoleFlags()
    {
        _fixture.MockAuthRepo.AddUser("kitchenuser", "pass", name: "Kitchen User", kitchenUser: true);

        using var client = _fixture.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/login", new UserAuthRequest { Username = "kitchenuser", Password = "pass" });

        var body = await response.Content.ReadFromJsonAsync<UserAuthResponse>();
        Assert.NotNull(body);
        Assert.True(body.KitchenUser);
        Assert.False(body.Admin);
    }

    // ── JWT token validation ────────────────────────────────────────

    [Fact]
    public async Task Login_Token_ContainsExpectedClaims()
    {
        _fixture.MockAuthRepo.AddUser("claimtest", "pass", name: "Claim Test", admin: true, mealsAdmin: true, mealsUser: true, kitchenUser: true, patientAdmin: true);

        using var client = _fixture.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/login", new UserAuthRequest { Username = "claimtest", Password = "pass" });
        var body = await response.Content.ReadFromJsonAsync<UserAuthResponse>();
        Assert.NotNull(body);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(body.AuthToken);

        Assert.Contains(jwt.Claims, c => c.Type == "username" && c.Value == "claimtest");
        Assert.Contains(jwt.Claims, c => c.Type == "admin");
        Assert.Contains(jwt.Claims, c => c.Type == "mealsAdmin");
        Assert.Contains(jwt.Claims, c => c.Type == "mealsUser");
        Assert.Contains(jwt.Claims, c => c.Type == "kitchenUser");
        Assert.Contains(jwt.Claims, c => c.Type == "patientAdmin");
    }

    [Fact]
    public async Task Login_Token_HasFutureExpiry()
    {
        _fixture.MockAuthRepo.AddUser("expirytest", "pass", name: "Expiry Test");

        using var client = _fixture.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/login", new UserAuthRequest { Username = "expirytest", Password = "pass" });
        var body = await response.Content.ReadFromJsonAsync<UserAuthResponse>();
        Assert.NotNull(body);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(body.AuthToken);

        Assert.True(jwt.ValidTo > DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_NonAdminUser_TokenDoesNotContainAdminClaim()
    {
        _fixture.MockAuthRepo.AddUser("regularuser", "pass", name: "Regular User", mealsUser: true);

        using var client = _fixture.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/login", new UserAuthRequest { Username = "regularuser", Password = "pass" });
        var body = await response.Content.ReadFromJsonAsync<UserAuthResponse>();
        Assert.NotNull(body);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(body.AuthToken);

        Assert.DoesNotContain(jwt.Claims, c => c.Type == "admin");
    }
}
