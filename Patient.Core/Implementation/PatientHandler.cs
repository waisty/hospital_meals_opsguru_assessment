using Hospital.Contracts;
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
            var patient = await _repo.GetPatientByIdAsync(guid, cancellationToken).ConfigureAwait(false);
            if (patient == null) return null;

            var allergyIds = await _repo.GetAllergyIdsByPatientIdAsync(guid, cancellationToken).ConfigureAwait(false);
            var clinicalStateIds = await _repo.GetClinicalStateIdsByPatientIdAsync(guid, cancellationToken).ConfigureAwait(false);

            return patient.ToPatientDetailViewModel(allergyIds, clinicalStateIds);
        }

        public async Task AddAllergyAsync(AllergyCreateRequest request, CancellationToken cancellationToken = default)
        {
            var allergy = new Allergy { Id = request.Id, Name = request.Name };
            await _repo.AddAllergyAsync(allergy, cancellationToken).ConfigureAwait(false);
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

        public async Task AddClinicalStateAsync(ClinicalStateCreateRequest request, CancellationToken cancellationToken = default)
        {
            var clinicalState = new ClinicalState { Id = request.Id, Name = request.Name };
            await _repo.AddClinicalStateAsync(clinicalState, cancellationToken).ConfigureAwait(false);
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

        public async Task AddDietTypeAsync(DietTypeCreateRequest request, CancellationToken cancellationToken = default)
        {
            var dietType = new DietType { Id = request.Id, Name = request.Name };
            await _repo.AddDietTypeAsync(dietType, cancellationToken).ConfigureAwait(false);
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
