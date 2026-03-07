using Hospital.Meals.ViewModels;

namespace Hospital.Meals.Core.Contracts;

/// <summary>
/// HTTP client for outbound calls from the Meals service to the Patient API.
/// </summary>
public interface IPatientApiClient
{
    /// <summary>
    /// Gets patient details from the Patient API (GET /api/v1/patients/{id}/detail).
    /// Returns null if the patient is not found (404).
    /// </summary>
    Task<PatientDetailViewModel?> GetPatientDetailAsync(string patientId, CancellationToken cancellationToken = default);
}
