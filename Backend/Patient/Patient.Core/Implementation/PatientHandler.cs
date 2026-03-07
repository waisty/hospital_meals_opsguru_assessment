using Hospital.Patient.Core.Contracts;
using Hospital.Patient.Core.InternalModels;
using Hospital.Patient.ViewModels;
using Hospital.Patient.ServiceViewModels;
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
                MobileNumber = request.MobileNumber,
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

        public async Task<PagedResult<PatientWithDietTypeNameViewModel>> ListPatientsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var paged = await _repo.ListPatientsAsync(page, pageSize, cancellationToken).ConfigureAwait(false);
            return new PagedResult<PatientWithDietTypeNameViewModel>
            {
                Items = paged.Items.Select(x => x.ToPatientWithDietTypeNameViewModel(x.DietTypeName)).ToList(),
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

            var allergies = await _repo.GetPatientAllergiesWithNameAsync(guid, cancellationToken).ConfigureAwait(false);
            var clinicalStates = await _repo.GetPatientClinicalStatesWithNameAsync(guid, cancellationToken).ConfigureAwait(false);

            return patient.ToPatientDetailViewModel(allergies, clinicalStates);
        }

        public async Task<PatientServiceDetailViewModel?> GetPatientServiceDetailByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(id, out var guid))
                return null;
            var patient = await _repo.GetPatientByIdAsync(guid, cancellationToken).ConfigureAwait(false);
            if (patient == null) return null;

            var allergies = await _repo.GetPatientAllergiesWithNameAsync(guid, cancellationToken).ConfigureAwait(false);
            var clinicalStates = await _repo.GetPatientClinicalStatesWithNameAsync(guid, cancellationToken).ConfigureAwait(false);

            return patient.ToPatientServiceDetailViewModel(
                allergies.Select(a => a.AllergyId).ToList(),
                clinicalStates.Select(cs => cs.ClinicalStateId).ToList());
        }

        public async Task<IReadOnlyList<string>> GetAllergyIdsByPatientIdAsync(string patientId, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(patientId, out var guid))
                return Array.Empty<string>();
            var allergies = await _repo.GetPatientAllergiesWithNameAsync(guid, cancellationToken).ConfigureAwait(false);
            return allergies.Select(a => a.AllergyId).ToList();
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

        public async Task<IReadOnlyList<string>> GetClinicalStateIdsByPatientIdAsync(string patientId, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(patientId, out var guid))
                return Array.Empty<string>();
            var clinicalStates = await _repo.GetPatientClinicalStatesWithNameAsync(guid, cancellationToken).ConfigureAwait(false);
            return clinicalStates.Select(cs => cs.ClinicalStateId).ToList();
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

        public async Task<bool> UpdatePatientAsync(string id, PatientUpdateRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null || !Guid.TryParse(id, out var guid))
                return false;
            var patient = await _repo.GetPatientByIdAsync(guid, cancellationToken).ConfigureAwait(false);
            if (patient is null)
                return false;
            patient.Name = request.Name;
            patient.MobileNumber = request.MobileNumber;
            patient.DietTypeId = request.DietTypeId;
            patient.Notes = request.Notes ?? "";
            await _repo.UpdatePatientAsync(patient, cancellationToken).ConfigureAwait(false);
            return true;
        }
    }
}
