using Hospital.Patient.ViewModels;

namespace Hospital.Patient.Core.Contracts;

public interface IAllergyHandler
{
    Task<string> AddAllergyAsync(AllergyCreateRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAllergyAsync(string id, AllergyUpdateRequest request, CancellationToken cancellationToken = default);
    Task<AllergyViewModel?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AllergyViewModel>> ListAllergiesAsync(CancellationToken cancellationToken = default);
}
