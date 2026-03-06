namespace Hospital.Patient.Core.Contracts
{
    /// <summary>
    /// HTTP client for outbound calls from the Patient service (e.g. to external APIs).
    /// </summary>
    public interface IMealsApiClient
    {
        Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default);
        Task<HttpResponseMessage> PostAsync(string? requestUri, HttpContent? content, CancellationToken cancellationToken = default);
    }
}
