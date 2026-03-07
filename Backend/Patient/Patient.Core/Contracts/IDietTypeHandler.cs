using Hospital.Patient.ViewModels;

namespace Hospital.Patient.Core.Contracts;

public interface IDietTypeHandler
{
    Task<string> AddDietTypeAsync(DietTypeCreateRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateDietTypeAsync(string id, DietTypeUpdateRequest request, CancellationToken cancellationToken = default);
    Task<DietTypeViewModel?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DietTypeViewModel>> ListDietTypesAsync(CancellationToken cancellationToken = default);
}
