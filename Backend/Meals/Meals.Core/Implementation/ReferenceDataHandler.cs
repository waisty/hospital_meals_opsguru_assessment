using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;

namespace Hospital.Meals.Core.Implementation
{
    internal sealed class ReferenceDataHandler : IReferenceDataHandler
    {
        private readonly IReferenceDataRepo _repo;

        public ReferenceDataHandler(IReferenceDataRepo repo)
        {
            _repo = repo;
        }

        public async Task AddAllergyAsync(AllergyCreateRequest request, CancellationToken cancellationToken = default)
        {
            var existing = await _repo.GetAllergyByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
            if (existing is not null)
                return;
            var allergy = new Allergy { Id = request.Id, Name = request.Name };
            await _repo.AddAllergyAsync(allergy, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> UpdateAllergyAsync(string id, AllergyUpdateRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            return await _repo.UpdateAllergyAsync(id, request.Name, cancellationToken).ConfigureAwait(false);
        }

        public async Task<AllergyViewModel?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var allergy = await _repo.GetAllergyByIdAsync(id, cancellationToken).ConfigureAwait(false);
            return allergy == null ? null : allergy.ToAllergyViewModel();
        }

        public async Task<IReadOnlyList<AllergyViewModel>> ListAllergiesAsync(CancellationToken cancellationToken = default)
        {
            var list = await _repo.ListAllergiesAsync(cancellationToken).ConfigureAwait(false);
            return list.Select(a => a.ToAllergyViewModel()).ToList();
        }

        public async Task AddClinicalStateAsync(ClinicalStateCreateRequest request, CancellationToken cancellationToken = default)
        {
            var existing = await _repo.GetClinicalStateByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
            if (existing is not null)
                return;
            var clinicalState = new ClinicalState { Id = request.Id, Name = request.Name };
            await _repo.AddClinicalStateAsync(clinicalState, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> UpdateClinicalStateAsync(string id, ClinicalStateUpdateRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null) return false;
            return await _repo.UpdateClinicalStateAsync(id, request.Name, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ClinicalStateViewModel?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var clinicalState = await _repo.GetClinicalStateByIdAsync(id, cancellationToken).ConfigureAwait(false);
            return clinicalState == null ? null : clinicalState.ToClinicalStateViewModel();
        }

        public async Task<IReadOnlyList<ClinicalStateViewModel>> ListClinicalStatesAsync(CancellationToken cancellationToken = default)
        {
            var list = await _repo.ListClinicalStatesAsync(cancellationToken).ConfigureAwait(false);
            return list.Select(c => c.ToClinicalStateViewModel()).ToList();
        }

        public async Task AddDietTypeAsync(DietTypeCreateRequest request, CancellationToken cancellationToken = default)
        {
            var existing = await _repo.GetDietTypeByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
            if (existing is not null)
                return;
            var dietType = new DietType { Id = request.Id, Name = request.Name };
            await _repo.AddDietTypeAsync(dietType, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> UpdateDietTypeAsync(string id, DietTypeUpdateRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null) return false;
            return await _repo.UpdateDietTypeAsync(id, request.Name, cancellationToken).ConfigureAwait(false);
        }

        public async Task<DietTypeViewModel?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var dietType = await _repo.GetDietTypeByIdAsync(id, cancellationToken).ConfigureAwait(false);
            return dietType == null ? null : dietType.ToDietTypeViewModel();
        }

        public async Task<IReadOnlyList<DietTypeViewModel>> ListDietTypesAsync(CancellationToken cancellationToken = default)
        {
            var list = await _repo.ListDietTypesAsync(cancellationToken).ConfigureAwait(false);
            return list.Select(d => d.ToDietTypeViewModel()).ToList();
        }
    }
}
