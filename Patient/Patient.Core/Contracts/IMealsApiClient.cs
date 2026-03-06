namespace Hospital.Patient.Core.Contracts
{
    /// <summary>
    /// HTTP client for outbound calls from the Patient service to the Meals API.
    /// </summary>
    public interface IMealsApiClient
    {
        //Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default);
        //Task<HttpResponseMessage> PostAsync(string? requestUri, HttpContent? content, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes a new allergy to the Meals API (POST /api/v1/allergies).
        /// </summary>
        Task<HttpResponseMessage> PublishAllergyAsync(string id, string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes an allergy update to the Meals API (PUT /api/v1/allergies/{id}).
        /// </summary>
        Task<HttpResponseMessage> PublishAllergyUpdateAsync(string id, string name, CancellationToken cancellationToken = default);
    }
}
