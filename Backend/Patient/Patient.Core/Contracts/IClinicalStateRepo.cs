using Hospital.Patient.Core.InternalModels;

namespace Hospital.Patient.Core.Contracts;

internal interface IClinicalStateRepo
{
    Task AddClinicalStateAndPublishAsync(ClinicalState clinicalState, CancellationToken cancellationToken = default);
    Task<bool> UpdateClinicalStateAndPublishAsync(string id, string name, CancellationToken cancellationToken = default);
    Task<ClinicalState?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ClinicalState>> ListClinicalStatesAsync(CancellationToken cancellationToken = default);
}
