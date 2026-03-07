using Hospital.Patient.Core.InternalModels;

namespace Hospital.Patient.Core.Contracts;

internal interface IDietTypeRepo
{
    Task AddDietTypeAndPublishAsync(DietType dietType, CancellationToken cancellationToken = default);
    Task<bool> UpdateDietTypeAndPublishAsync(string id, string name, CancellationToken cancellationToken = default);
    Task<DietType?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DietType>> ListDietTypesAsync(CancellationToken cancellationToken = default);
}
