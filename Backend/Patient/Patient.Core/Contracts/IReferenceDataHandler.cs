using Hospital.Patient.ViewModels;

namespace Hospital.Patient.Core.Contracts
{
    public interface IReferenceDataHandler
    {
        Task<string> AddAllergyAsync(AllergyCreateRequest request, CancellationToken cancellationToken = default);
        Task<bool> UpdateAllergyAsync(string id, AllergyUpdateRequest request, CancellationToken cancellationToken = default);
        Task<AllergyViewModel?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AllergyViewModel>> ListAllergiesAsync(CancellationToken cancellationToken = default);

        Task<string> AddClinicalStateAsync(ClinicalStateCreateRequest request, CancellationToken cancellationToken = default);
        Task<bool> UpdateClinicalStateAsync(string id, ClinicalStateUpdateRequest request, CancellationToken cancellationToken = default);
        Task<ClinicalStateViewModel?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ClinicalStateViewModel>> ListClinicalStatesAsync(CancellationToken cancellationToken = default);

        Task<string> AddDietTypeAsync(DietTypeCreateRequest request, CancellationToken cancellationToken = default);
        Task<bool> UpdateDietTypeAsync(string id, DietTypeUpdateRequest request, CancellationToken cancellationToken = default);
        Task<DietTypeViewModel?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DietTypeViewModel>> ListDietTypesAsync(CancellationToken cancellationToken = default);
    }
}
