using Hospital.Patient.Core.InternalModels;

namespace Hospital.Patient.Core.Contracts;

internal interface IAllergyRepo
{
    Task AddAllergyAndPublishAsync(Allergy allergy, CancellationToken cancellationToken = default);
    Task<bool> UpdateAllergyAndPublishAsync(string id, string name, CancellationToken cancellationToken = default);
    Task<Allergy?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Allergy>> ListAllergiesAsync(CancellationToken cancellationToken = default);
}
