using Patient.UIViewModels;

namespace Patient.Core.Contracts
{
    internal interface IPatientHandler
    {
        // Patient
        Task AddPatientAsync(PatientCreateRequest request, CancellationToken cancellationToken = default);
        Task<PatientViewModel?> GetPatientByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResultViewModel<PatientViewModel>> ListPatientsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<PatientDetailViewModel?> GetPatientDetailByIdAsync(string id, CancellationToken cancellationToken = default);

        // Allergy
        Task AddAllergyAsync(AllergyCreateRequest request, CancellationToken cancellationToken = default);
        Task<AllergyViewModel?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AllergyViewModel>> ListAllergiesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetAllergyIdsByPatientIdAsync(string patientId, CancellationToken cancellationToken = default);

        // Clinical state
        Task AddClinicalStateAsync(ClinicalStateCreateRequest request, CancellationToken cancellationToken = default);
        Task<ClinicalStateViewModel?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ClinicalStateViewModel>> ListClinicalStatesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetClinicalStateIdsByPatientIdAsync(string patientId, CancellationToken cancellationToken = default);

        // Diet type
        Task AddDietTypeAsync(DietTypeCreateRequest request, CancellationToken cancellationToken = default);
        Task<DietTypeViewModel?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DietTypeViewModel>> ListDietTypesAsync(CancellationToken cancellationToken = default);
    }
}
