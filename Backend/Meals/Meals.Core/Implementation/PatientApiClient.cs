using System.Text.Json;
using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;
using Microsoft.Extensions.Configuration;

namespace Hospital.Meals.Core.Implementation;

internal sealed class PatientApiClient : IPatientApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private const string PatientDetailPath = "patients";

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public PatientApiClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<PatientDetailViewModel?> GetPatientDetailAsync(string patientId, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{PatientDetailPath}/{patientId}/service-detail");
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<PatientDetailViewModel>(json, JsonOptions);
    }
}
