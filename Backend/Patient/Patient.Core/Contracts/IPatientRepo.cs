using Hospital.Patient.Core.InternalModels;

namespace Hospital.Patient.Core.Contracts
{
    internal interface IPatientRepo
    {
        Task AddPatientAsync(InternalModels.Patient patient, CancellationToken cancellationToken = default);
        Task<InternalModels.Patient?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<InternalModels.Patient>> ListPatientsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PatientAllergyWithName>> GetPatientAllergiesWithNameAsync(Guid patientId, CancellationToken cancellationToken = default);
        Task SetAllergyIdsForPatientAsync(Guid patientId, IReadOnlyList<string> allergyIds, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PatientClinicalStateWithName>> GetPatientClinicalStatesWithNameAsync(Guid patientId, CancellationToken cancellationToken = default);
        Task SetClinicalStateIdsForPatientAsync(Guid patientId, IReadOnlyList<string> clinicalStateIds, CancellationToken cancellationToken = default);
    }
}
