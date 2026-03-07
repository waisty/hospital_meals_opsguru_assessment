using Hospital.Patient.Core.InternalModels;

namespace Hospital.Patient.Core.Contracts
{
    internal interface IPatientRepo
    {
        Task AddPatientAsync(InternalModels.Patient patient, CancellationToken cancellationToken = default);
        Task<InternalModels.Patient?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<InternalModels.PatientWithDietTypeName>> ListPatientsAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PatientAllergyWithName>> GetPatientAllergiesWithNameAsync(Guid patientId, CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<Guid, IReadOnlyList<string>>> GetPatientAllergyNamesByPatientIdsAsync(IReadOnlyList<Guid> patientIds, CancellationToken cancellationToken = default);
        Task SetAllergyIdsForPatientAsync(Guid patientId, IReadOnlyList<string> allergyIds, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PatientClinicalStateWithName>> GetPatientClinicalStatesWithNameAsync(Guid patientId, CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<Guid, IReadOnlyList<string>>> GetPatientClinicalStateNamesByPatientIdsAsync(IReadOnlyList<Guid> patientIds, CancellationToken cancellationToken = default);
        Task SetClinicalStateIdsForPatientAsync(Guid patientId, IReadOnlyList<string> clinicalStateIds, CancellationToken cancellationToken = default);
        Task UpdatePatientAsync(InternalModels.Patient patient, CancellationToken cancellationToken = default);
    }
}
