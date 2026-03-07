using Hospital.Patient.ViewModels;

namespace Hospital.Patient.Core.Contracts;

public interface IClinicalStateHandler
{
    Task<string> AddClinicalStateAsync(ClinicalStateCreateRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateClinicalStateAsync(string id, ClinicalStateUpdateRequest request, CancellationToken cancellationToken = default);
    Task<ClinicalStateViewModel?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ClinicalStateViewModel>> ListClinicalStatesAsync(CancellationToken cancellationToken = default);
}
