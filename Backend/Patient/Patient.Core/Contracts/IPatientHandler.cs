using Hospital.Patient.ViewModels;
using Hospital.Patient.ServiceViewModels;

namespace Hospital.Patient.Core.Contracts
{
    public interface IPatientHandler
    {
        // Patient
        Task<Guid> AddPatientAsync(PatientCreateRequest request, CancellationToken cancellationToken = default);
        Task<PatientViewModel?> GetPatientByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<PatientViewModel>> ListPatientsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<PatientDetailViewModel?> GetPatientDetailByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PatientServiceDetailViewModel?> GetPatientServiceDetailByIdAsync(string id, CancellationToken cancellationToken = default);

        // Allergy
        Task<string> AddAllergyAsync(AllergyCreateRequest request, CancellationToken cancellationToken = default);
        Task<bool> UpdateAllergyAsync(string id, AllergyUpdateRequest request, CancellationToken cancellationToken = default);
        Task<AllergyViewModel?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AllergyViewModel>> ListAllergiesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetAllergyIdsByPatientIdAsync(string patientId, CancellationToken cancellationToken = default);
        Task<bool> UpdatePatientAllergiesAsync(string patientId, PatientAllergiesUpdateRequest request, CancellationToken cancellationToken = default);

        // Clinical state
        Task<string> AddClinicalStateAsync(ClinicalStateCreateRequest request, CancellationToken cancellationToken = default);
        Task<bool> UpdateClinicalStateAsync(string id, ClinicalStateUpdateRequest request, CancellationToken cancellationToken = default);
        Task<ClinicalStateViewModel?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ClinicalStateViewModel>> ListClinicalStatesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetClinicalStateIdsByPatientIdAsync(string patientId, CancellationToken cancellationToken = default);
        Task<bool> UpdatePatientClinicalStatesAsync(string patientId, PatientClinicalStatesUpdateRequest request, CancellationToken cancellationToken = default);

        // Diet type
        Task<string> AddDietTypeAsync(DietTypeCreateRequest request, CancellationToken cancellationToken = default);
        Task<bool> UpdateDietTypeAsync(string id, DietTypeUpdateRequest request, CancellationToken cancellationToken = default);
        Task<DietTypeViewModel?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DietTypeViewModel>> ListDietTypesAsync(CancellationToken cancellationToken = default);
    }
}
