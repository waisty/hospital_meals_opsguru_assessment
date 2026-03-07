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

        /// <summary>
        /// Publishes a new clinical state to the Meals API (POST /api/v1/clinical-states).
        /// </summary>
        Task<HttpResponseMessage> PublishClinicalStateAsync(string id, string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes a clinical state update to the Meals API (PUT /api/v1/clinical-states/{id}).
        /// </summary>
        Task<HttpResponseMessage> PublishClinicalStateUpdateAsync(string id, string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes a new diet type to the Meals API (POST /api/v1/diet-types).
        /// </summary>
        Task<HttpResponseMessage> PublishDietTypeAsync(string id, string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes a diet type update to the Meals API (PUT /api/v1/diet-types/{id}).
        /// </summary>
        Task<HttpResponseMessage> PublishDietTypeUpdateAsync(string id, string name, CancellationToken cancellationToken = default);
    }
}
