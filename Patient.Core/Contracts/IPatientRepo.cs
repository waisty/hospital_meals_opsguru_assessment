using Patient.Core.InternalModels;
namespace Patient.Core.Contracts
{
    internal interface IPatientRepo
    {
        // Patient
        Task AddPatientAsync(InternalModels.Patient patient, CancellationToken cancellationToken = default);
        Task<InternalModels.Patient?> GetPatientByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<InternalModels.Patient>> ListPatientsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        // Allergy
        Task AddAllergyAsync(Allergy allergy, CancellationToken cancellationToken = default);
        Task<Allergy?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Allergy>> ListAllergiesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetAllergyIdsByPatientIdAsync(string patientId, CancellationToken cancellationToken = default);

        // Clinical state
        Task AddClinicalStateAsync(ClinicalState clinicalState, CancellationToken cancellationToken = default);
        Task<ClinicalState?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ClinicalState>> ListClinicalStatesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetClinicalStateIdsByPatientIdAsync(string patientId, CancellationToken cancellationToken = default);

        // Diet type
        Task AddDietTypeAsync(DietType dietType, CancellationToken cancellationToken = default);
        Task<DietType?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DietType>> ListDietTypesAsync(CancellationToken cancellationToken = default);
    }
}
