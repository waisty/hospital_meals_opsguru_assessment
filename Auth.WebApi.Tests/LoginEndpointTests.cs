using System.Net;
using System.Net.Http.Json;
using Auth.UIViewModels;
using Xunit;

namespace Auth.WebApi.Tests;

public sealed class LoginEndpointTests : IClassFixture<AuthWebApiFixture>
{
    private readonly AuthWebApiFixture _fixture;

    public LoginEndpointTests(AuthWebApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200_AndToken()
    {
        _fixture.MockAuthRepo.AddUser("testuser", "testpass", name: "Test User", admin: true);

        using var client = _fixture.CreateClient();
        var request = new UserAuthRequest { Username = "testuser", Password = "testpass" };
        var response = await client.PostAsJsonAsync("/login", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<UserAuthResponse>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrEmpty(body.AuthToken));
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_Returns401()
    {
        _fixture.MockAuthRepo.AddUser("validuser", "validpass");
        // Request with wrong password

        using var client = _fixture.CreateClient();
        var request = new UserAuthRequest { Username = "validuser", Password = "wrongpass" };
        var response = await client.PostAsJsonAsync("/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithUnknownUser_Returns401()
    {
        // No user added to MockAuthRepo

        using var client = _fixture.CreateClient();
        var request = new UserAuthRequest { Username = "nobody", Password = "any" };
        var response = await client.PostAsJsonAsync("/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
