using System.Net;
using System.Text;
using System.Text.Json;
using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;
using Microsoft.Extensions.Configuration;

namespace Hospital.Meals.Core.Implementation;

internal sealed class KitchenApiClient : IKitchenApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private const string TraysPath = "trays";

    private readonly HttpClient _httpClient;

    public KitchenApiClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _ = configuration;
    }

    public async Task<Guid> PublishTrayAsync(KitchenPublishTrayRequest request, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(request, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(TraysPath, content, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new InvalidOperationException("Kitchen service returned 404. Check KitchenAPIEndpoint configuration.");

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var parsed = JsonSerializer.Deserialize<KitchenPublishTrayResponse>(responseJson, JsonOptions);
        if (parsed is null || parsed.Id == default)
            throw new InvalidOperationException("Kitchen service did not return a tray id.");
        return parsed.Id;
    }

    private sealed class KitchenPublishTrayResponse
    {
        public Guid Id { get; set; }
    }
}
