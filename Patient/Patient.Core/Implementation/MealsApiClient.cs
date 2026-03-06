using Hospital.Patient.Core.Contracts;

namespace Hospital.Patient.Core.Implementation
{
    internal sealed class MealsApiClient : IMealsApiClient
    {
        private readonly HttpClient _httpClient;

        public MealsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default) =>
            _httpClient.GetAsync(requestUri, cancellationToken);

        public Task<HttpResponseMessage> PostAsync(string? requestUri, HttpContent? content, CancellationToken cancellationToken = default) =>
            _httpClient.PostAsync(requestUri, content, cancellationToken);
    }
}
