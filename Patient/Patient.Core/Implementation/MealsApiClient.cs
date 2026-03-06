using System.Net.Http.Headers;
using Hospital.Patient.Core.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Hospital.Patient.Core.Implementation
{
    internal sealed class MealsApiClient : IMealsApiClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        private const string AllergiesPath = "allergies";
        private const string ClinicalStatesPath = "clinical-states";
        private const string DietTypesPath = "diet-types";

        private readonly HttpClient _httpClient;
        private readonly IConfiguration configuration;

        public MealsApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            this.configuration = configuration;
        }

        public Task<HttpResponseMessage> PublishAllergyAsync(string id, string name, CancellationToken cancellationToken = default)
        {
            var payload = new { Id = id, Name = name };
            var json = JsonSerializer.Serialize(payload, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, AllergiesPath) { Content = content };
            return _httpClient.SendAsync(request, cancellationToken);
        }

        public Task<HttpResponseMessage> PublishAllergyUpdateAsync(string id, string name, CancellationToken cancellationToken = default)
        {
            var payload = new { Name = name };
            var json = JsonSerializer.Serialize(payload, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, $"{AllergiesPath}/{id}") { Content = content };
            return _httpClient.SendAsync(request, cancellationToken);
        }

        public Task<HttpResponseMessage> PublishClinicalStateAsync(string id, string name, CancellationToken cancellationToken = default)
        {
            var payload = new { Id = id, Name = name };
            var json = JsonSerializer.Serialize(payload, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, ClinicalStatesPath) { Content = content };
            return _httpClient.SendAsync(request, cancellationToken);
        }

        public Task<HttpResponseMessage> PublishClinicalStateUpdateAsync(string id, string name, CancellationToken cancellationToken = default)
        {
            var payload = new { Name = name };
            var json = JsonSerializer.Serialize(payload, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, $"{ClinicalStatesPath}/{id}") { Content = content };
            return _httpClient.SendAsync(request, cancellationToken);
        }

        public Task<HttpResponseMessage> PublishDietTypeAsync(string id, string name, CancellationToken cancellationToken = default)
        {
            var payload = new { Id = id, Name = name };
            var json = JsonSerializer.Serialize(payload, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, DietTypesPath) { Content = content };
            return _httpClient.SendAsync(request, cancellationToken);
        }

        public Task<HttpResponseMessage> PublishDietTypeUpdateAsync(string id, string name, CancellationToken cancellationToken = default)
        {
            var payload = new { Name = name };
            var json = JsonSerializer.Serialize(payload, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, $"{DietTypesPath}/{id}") { Content = content };
            return _httpClient.SendAsync(request, cancellationToken);
        }
    }
}
