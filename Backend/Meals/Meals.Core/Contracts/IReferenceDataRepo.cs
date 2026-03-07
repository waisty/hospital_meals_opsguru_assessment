using Hospital.Meals.Core.InternalModels;

namespace Hospital.Meals.Core.Contracts
{
    internal interface IReferenceDataRepo
    {
        Task AddAllergyAsync(Allergy allergy, CancellationToken cancellationToken = default);
        Task<bool> UpdateAllergyAsync(string id, string name, CancellationToken cancellationToken = default);
        Task<Allergy?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Allergy>> ListAllergiesAsync(CancellationToken cancellationToken = default);

        Task AddClinicalStateAsync(ClinicalState clinicalState, CancellationToken cancellationToken = default);
        Task<bool> UpdateClinicalStateAsync(string id, string name, CancellationToken cancellationToken = default);
        Task<ClinicalState?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ClinicalState>> ListClinicalStatesAsync(CancellationToken cancellationToken = default);

        Task AddDietTypeAsync(DietType dietType, CancellationToken cancellationToken = default);
        Task<bool> UpdateDietTypeAsync(string id, string name, CancellationToken cancellationToken = default);
        Task<DietType?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DietType>> ListDietTypesAsync(CancellationToken cancellationToken = default);
    }
}
