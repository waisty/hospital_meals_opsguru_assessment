using Hospital.Patient.Core.Contracts;
using Hospital.Patient.Core.InternalModels;
using Hospital.Patient.ViewModels;
using System.Text.RegularExpressions;

namespace Hospital.Patient.Core.Implementation
{
    internal sealed class ReferenceDataHandler : IReferenceDataHandler
    {
        private readonly IReferenceDataRepo _repo;

        public ReferenceDataHandler(IReferenceDataRepo repo)
        {
            _repo = repo;
        }

        public async Task<string> AddAllergyAsync(AllergyCreateRequest request, CancellationToken cancellationToken = default)
        {
            var allergy = new Allergy { Id = CreateReadableIdFromName(request.Name), Name = request.Name };
            await _repo.AddAllergyAndPublishAsync(allergy, cancellationToken).ConfigureAwait(false);
            return allergy.Id;
        }

        public async Task<bool> UpdateAllergyAsync(string id, AllergyUpdateRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null) return false;
            return await _repo.UpdateAllergyAndPublishAsync(id, request.Name, cancellationToken).ConfigureAwait(false);
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

        public async Task<string> AddClinicalStateAsync(ClinicalStateCreateRequest request, CancellationToken cancellationToken = default)
        {
            var clinicalState = new ClinicalState { Id = CreateReadableIdFromName(request.Name), Name = request.Name };
            await _repo.AddClinicalStateAndPublishAsync(clinicalState, cancellationToken).ConfigureAwait(false);
            return clinicalState.Id;
        }

        public async Task<bool> UpdateClinicalStateAsync(string id, ClinicalStateUpdateRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null) return false;
            return await _repo.UpdateClinicalStateAndPublishAsync(id, request.Name, cancellationToken).ConfigureAwait(false);
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

        public async Task<string> AddDietTypeAsync(DietTypeCreateRequest request, CancellationToken cancellationToken = default)
        {
            var dietType = new DietType { Id = CreateReadableIdFromName(request.Name), Name = request.Name };
            await _repo.AddDietTypeAndPublishAsync(dietType, cancellationToken).ConfigureAwait(false);
            return dietType.Id;
        }

        public async Task<bool> UpdateDietTypeAsync(string id, DietTypeUpdateRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null) return false;
            return await _repo.UpdateDietTypeAndPublishAsync(id, request.Name, cancellationToken).ConfigureAwait(false);
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

        private static string CreateReadableIdFromName(string name)
        {
            var cleaned = Regex.Replace(name, @"[^A-Za-z0-9]", "").ToUpperInvariant();
            var truncated = cleaned.Length > 20 ? cleaned[..20] : cleaned;
            var suffix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()[^5..];
            return $"{truncated}_{suffix}";
        }
    }
}
