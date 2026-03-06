using Patient.Core.Contracts;
using Patient.Core.InternalModels;
using Patient.UIViewModels;
using PatientEntity = Patient.Core.InternalModels.Patient;

namespace Patient.Core.Implementation
{
    internal sealed class PatientHandler : IPatientHandler
    {
        private readonly IPatientRepo _repo;

        public PatientHandler(IPatientRepo repo)
        {
            _repo = repo;
        }

        public async Task AddPatientAsync(PatientCreateRequest request, CancellationToken cancellationToken = default)
        {
            var patient = new PatientEntity
            {
                Id = request.Id,
                Name = request.Name,
                DietTypeId = request.DietTypeId,
                Notes = request.Notes ?? ""
            };
            await _repo.AddPatientAsync(patient, cancellationToken).ConfigureAwait(false);
        }

        public async Task<PatientViewModel?> GetPatientByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var patient = await _repo.GetPatientByIdAsync(id, cancellationToken).ConfigureAwait(false);
            return patient == null ? null : ToPatientViewModel(patient);
        }

        public async Task<PagedResultViewModel<PatientViewModel>> ListPatientsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var paged = await _repo.ListPatientsAsync(page, pageSize, cancellationToken).ConfigureAwait(false);
            return new PagedResultViewModel<PatientViewModel>
            {
                Items = paged.Items.Select(ToPatientViewModel).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<PatientDetailViewModel?> GetPatientDetailByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var patient = await _repo.GetPatientByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (patient == null) return null;

            var allergyIds = await _repo.GetAllergyIdsByPatientIdAsync(id, cancellationToken).ConfigureAwait(false);
            var clinicalStateIds = await _repo.GetClinicalStateIdsByPatientIdAsync(id, cancellationToken).ConfigureAwait(false);

            return new PatientDetailViewModel
            {
                Id = patient.Id,
                Name = patient.Name,
                DietTypeId = patient.DietTypeId,
                Notes = patient.Notes,
                AllergyIds = allergyIds,
                ClinicalStateIds = clinicalStateIds
            };
        }

        public async Task AddAllergyAsync(AllergyCreateRequest request, CancellationToken cancellationToken = default)
        {
            var allergy = new Allergy { Id = request.Id, Name = request.Name };
            await _repo.AddAllergyAsync(allergy, cancellationToken).ConfigureAwait(false);
        }

        public async Task<AllergyViewModel?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var allergy = await _repo.GetAllergyByIdAsync(id, cancellationToken).ConfigureAwait(false);
            return allergy == null ? null : ToAllergyViewModel(allergy);
        }

        public async Task<IReadOnlyList<AllergyViewModel>> ListAllergiesAsync(CancellationToken cancellationToken = default)
        {
            var list = await _repo.ListAllergiesAsync(cancellationToken).ConfigureAwait(false);
            return list.Select(ToAllergyViewModel).ToList();
        }

        public async Task<IReadOnlyList<string>> GetAllergyIdsByPatientIdAsync(string patientId, CancellationToken cancellationToken = default)
        {
            return await _repo.GetAllergyIdsByPatientIdAsync(patientId, cancellationToken).ConfigureAwait(false);
        }

        public async Task AddClinicalStateAsync(ClinicalStateCreateRequest request, CancellationToken cancellationToken = default)
        {
            var clinicalState = new ClinicalState { Id = request.Id, Name = request.Name };
            await _repo.AddClinicalStateAsync(clinicalState, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ClinicalStateViewModel?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var clinicalState = await _repo.GetClinicalStateByIdAsync(id, cancellationToken).ConfigureAwait(false);
            return clinicalState == null ? null : ToClinicalStateViewModel(clinicalState);
        }

        public async Task<IReadOnlyList<ClinicalStateViewModel>> ListClinicalStatesAsync(CancellationToken cancellationToken = default)
        {
            var list = await _repo.ListClinicalStatesAsync(cancellationToken).ConfigureAwait(false);
            return list.Select(ToClinicalStateViewModel).ToList();
        }

        public async Task<IReadOnlyList<string>> GetClinicalStateIdsByPatientIdAsync(string patientId, CancellationToken cancellationToken = default)
        {
            return await _repo.GetClinicalStateIdsByPatientIdAsync(patientId, cancellationToken).ConfigureAwait(false);
        }

        public async Task AddDietTypeAsync(DietTypeCreateRequest request, CancellationToken cancellationToken = default)
        {
            var dietType = new DietType { Id = request.Id, Name = request.Name };
            await _repo.AddDietTypeAsync(dietType, cancellationToken).ConfigureAwait(false);
        }

        public async Task<DietTypeViewModel?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var dietType = await _repo.GetDietTypeByIdAsync(id, cancellationToken).ConfigureAwait(false);
            return dietType == null ? null : ToDietTypeViewModel(dietType);
        }

        public async Task<IReadOnlyList<DietTypeViewModel>> ListDietTypesAsync(CancellationToken cancellationToken = default)
        {
            var list = await _repo.ListDietTypesAsync(cancellationToken).ConfigureAwait(false);
            return list.Select(ToDietTypeViewModel).ToList();
        }

        private static PatientViewModel ToPatientViewModel(PatientEntity p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            DietTypeId = p.DietTypeId,
            Notes = p.Notes
        };

        private static AllergyViewModel ToAllergyViewModel(Allergy a) => new()
        {
            Id = a.Id,
            Name = a.Name
        };

        private static ClinicalStateViewModel ToClinicalStateViewModel(ClinicalState c) => new()
        {
            Id = c.Id,
            Name = c.Name
        };

        private static DietTypeViewModel ToDietTypeViewModel(DietType d) => new()
        {
            Id = d.Id,
            Name = d.Name
        };
    }
}
