using Hospital.Patient.Core.Contracts;
using Hospital.Patient.Core.InternalModels;
using Hospital.Patient.UIViewModels;
using PatientEntity = Hospital.Patient.Core.InternalModels.Patient;

namespace Hospital.Patient.Core.Implementation
{
    internal sealed class PatientHandler : IPatientHandler
    {
        private readonly IPatientRepo _repo;

        public PatientHandler(IPatientRepo repo)
        {
            _repo = repo;
        }

        public async Task<Guid> AddPatientAsync(PatientCreateRequest request, CancellationToken cancellationToken = default)
        {
            var patient = new PatientEntity
            {
                Name = request.Name,
                DietTypeId = request.DietTypeId,
                Notes = request.Notes ?? ""
            };
            await _repo.AddPatientAsync(patient, cancellationToken).ConfigureAwait(false);
            return patient.Id;
        }

        public async Task<PatientViewModel?> GetPatientByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(id, out var guid))
                return null;
            var patient = await _repo.GetPatientByIdAsync(guid, cancellationToken).ConfigureAwait(false);
            return patient == null ? null : patient.ToPatientViewModel();
        }

        public async Task<PagedResult<PatientViewModel>> ListPatientsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var paged = await _repo.ListPatientsAsync(page, pageSize, cancellationToken).ConfigureAwait(false);
            return new PagedResult<PatientViewModel>
            {
                Items = paged.Items.Select(p => p.ToPatientViewModel()).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<PatientDetailViewModel?> GetPatientDetailByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(id, out var guid))
                return null;
            var getPatientTask = _repo.GetPatientByIdAsync(guid, cancellationToken);
            var getAllergyIdsTask = _repo.GetAllergyIdsByPatientIdAsync(guid, cancellationToken);
            var getClinicalStateIdsTask = _repo.GetClinicalStateIdsByPatientIdAsync(guid, cancellationToken);

            Task[] tasks = [getPatientTask, getAllergyIdsTask, getClinicalStateIdsTask];

            await Task.WhenAll(tasks);

            var patient = getPatientTask.GetAwaiter().GetResult();
            if (patient == null) return null;

            var allergyIds = getAllergyIdsTask.GetAwaiter().GetResult();
            var clinicalStateIds = getClinicalStateIdsTask.GetAwaiter().GetResult();

            return patient.ToPatientDetailViewModel(allergyIds, clinicalStateIds);
        }

        public async Task<string> AddAllergyAsync(AllergyCreateRequest request, CancellationToken cancellationToken = default)
        {
            var allergy = new Allergy { Id = CreateReadableIdFromName(request.Name), Name = request.Name };
            await _repo.AddAllergyAsync(allergy, cancellationToken).ConfigureAwait(false);
            return allergy.Id;
        }

        public async Task<bool> UpdateAllergyAsync(string id, AllergyUpdateRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null) return false;
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

        public async Task<IReadOnlyList<string>> GetAllergyIdsByPatientIdAsync(string patientId, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(patientId, out var guid))
                return Array.Empty<string>();
            return await _repo.GetAllergyIdsByPatientIdAsync(guid, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> UpdatePatientAllergiesAsync(string patientId, PatientAllergiesUpdateRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null || !Guid.TryParse(patientId, out var guid))
                return false;
            var patient = await _repo.GetPatientByIdAsync(guid, cancellationToken).ConfigureAwait(false);
            if (patient is null)
                return false;
            await _repo.SetAllergyIdsForPatientAsync(guid, request.AllergyIds ?? [], cancellationToken).ConfigureAwait(false);
            return true;
        }

        public async Task<string> AddClinicalStateAsync(ClinicalStateCreateRequest request, CancellationToken cancellationToken = default)
        {
            var clinicalState = new ClinicalState { Id = CreateReadableIdFromName(request.Name), Name = request.Name };
            await _repo.AddClinicalStateAsync(clinicalState, cancellationToken).ConfigureAwait(false);
            return clinicalState.Id;
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

        public async Task<IReadOnlyList<string>> GetClinicalStateIdsByPatientIdAsync(string patientId, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(patientId, out var guid))
                return Array.Empty<string>();
            return await _repo.GetClinicalStateIdsByPatientIdAsync(guid, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> UpdatePatientClinicalStatesAsync(string patientId, PatientClinicalStatesUpdateRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null || !Guid.TryParse(patientId, out var guid))
                return false;
            var patient = await _repo.GetPatientByIdAsync(guid, cancellationToken).ConfigureAwait(false);
            if (patient is null)
                return false;
            await _repo.SetClinicalStateIdsForPatientAsync(guid, request.ClinicalStateIds ?? [], cancellationToken).ConfigureAwait(false);
            return true;
        }

        public async Task<string> AddDietTypeAsync(DietTypeCreateRequest request, CancellationToken cancellationToken = default)
        {
            var dietType = new DietType { Id = CreateReadableIdFromName(request.Name), Name = request.Name };
            await _repo.AddDietTypeAsync(dietType, cancellationToken).ConfigureAwait(false);
            return dietType.Id;
        }

        public async Task<bool> UpdateDietTypeAsync(string id, DietTypeUpdateRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null) return false;
            return await _repo.UpdateDietTypeAsync(id, request.Name, cancellationToken).ConfigureAwait(false);
        }

        private static string CreateReadableIdFromName(string name)
        {
            var cleaned = System.Text.RegularExpressions.Regex.Replace(name, @"[^A-Za-z0-9]", "").ToUpperInvariant();
            var truncated = cleaned.Length > 20 ? cleaned[..20] : cleaned;
            var suffix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()[^5..];
            return truncated + suffix;
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
