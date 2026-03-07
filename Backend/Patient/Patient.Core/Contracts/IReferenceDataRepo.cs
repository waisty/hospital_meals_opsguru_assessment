using Hospital.Patient.Core.InternalModels;

namespace Hospital.Patient.Core.Contracts
{
    internal interface IReferenceDataRepo
    {
        Task AddAllergyAndPublishAsync(Allergy allergy, CancellationToken cancellationToken = default);
        Task<bool> UpdateAllergyAndPublishAsync(string id, string name, CancellationToken cancellationToken = default);
        Task<Allergy?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Allergy>> ListAllergiesAsync(CancellationToken cancellationToken = default);

        Task AddClinicalStateAndPublishAsync(ClinicalState clinicalState, CancellationToken cancellationToken = default);
        Task<bool> UpdateClinicalStateAndPublishAsync(string id, string name, CancellationToken cancellationToken = default);
        Task<ClinicalState?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ClinicalState>> ListClinicalStatesAsync(CancellationToken cancellationToken = default);

        Task AddDietTypeAndPublishAsync(DietType dietType, CancellationToken cancellationToken = default);
        Task<bool> UpdateDietTypeAndPublishAsync(string id, string name, CancellationToken cancellationToken = default);
        Task<DietType?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DietType>> ListDietTypesAsync(CancellationToken cancellationToken = default);
    }
}
