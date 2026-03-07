using Hospital.Patient.ViewModels;
using Hospital.Patient.ServiceViewModels;

namespace Hospital.Patient.Core.Contracts
{
    public interface IPatientHandler
    {
        Task<Guid> AddPatientAsync(PatientCreateRequest request, CancellationToken cancellationToken = default);
        Task<PatientViewModel?> GetPatientByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<PatientWithDietTypeNameViewModel>> ListPatientsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<PatientDetailViewModel?> GetPatientDetailByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PatientServiceDetailViewModel?> GetPatientServiceDetailByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetAllergyIdsByPatientIdAsync(string patientId, CancellationToken cancellationToken = default);
        Task<BatchPatientAllergiesResponse> GetAllergiesByPatientIdsAsync(BatchPatientAllergiesRequest request, CancellationToken cancellationToken = default);
        Task<bool> UpdatePatientAllergiesAsync(string patientId, PatientAllergiesUpdateRequest request, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetClinicalStateIdsByPatientIdAsync(string patientId, CancellationToken cancellationToken = default);
        Task<BatchPatientClinicalStatesResponse> GetClinicalStatesByPatientIdsAsync(BatchPatientClinicalStatesRequest request, CancellationToken cancellationToken = default);
        Task<bool> UpdatePatientClinicalStatesAsync(string patientId, PatientClinicalStatesUpdateRequest request, CancellationToken cancellationToken = default);
        Task<bool> UpdatePatientAsync(string id, PatientUpdateRequest request, CancellationToken cancellationToken = default);
    }
}
