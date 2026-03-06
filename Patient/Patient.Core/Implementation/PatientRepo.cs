using Microsoft.EntityFrameworkCore;
using Hospital.Patient.Core.Contracts;
using Hospital.Patient.Core.InternalModels;

namespace Hospital.Patient.Core.Implementation
{
    internal sealed class PatientRepo : IPatientRepo
    {
        private readonly PatientDBContext _context;
        private readonly IMealsApiClient _mealsApiClient;

        public PatientRepo(PatientDBContext context, IMealsApiClient mealsApiClient)
        {
            _context = context;
            _mealsApiClient = mealsApiClient;
        }

        public async Task AddPatientAsync(InternalModels.Patient patient, CancellationToken cancellationToken = default)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<InternalModels.Patient?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<PagedResult<InternalModels.Patient>> ListPatientsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await _context.Patients.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await _context.Patients
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return new PagedResult<InternalModels.Patient>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task AddAllergyAsync(Allergy allergy, CancellationToken cancellationToken = default)
        {
            _context.Allergies.Add(allergy);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task AddAllergyAndPublishAsync(Allergy allergy, CancellationToken cancellationToken = default)
        {
            await ExecuteInTransactionAsync(async ct =>
            {
                //executing it in a transacton to make sure we don't commit until the service confirms receipt
                await AddAllergyAsync(allergy, ct).ConfigureAwait(false);
                var response = await _mealsApiClient.PublishAllergyAsync(allergy.Id, allergy.Name, ct).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> UpdateAllergyAsync(string id, string name, CancellationToken cancellationToken = default)
        {
            var allergy = await _context.Allergies.FirstOrDefaultAsync(a => a.Id == id, cancellationToken).ConfigureAwait(false);
            if (allergy is null) return false;
            allergy.Name = name;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            var response = await _mealsApiClient.PublishAllergyUpdateAsync(id, name, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<Allergy?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Allergies
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<Allergy>> ListAllergiesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Allergies
                .OrderBy(a => a.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<string>> GetAllergyIdsByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            return await _context.PatientAllergies
                .Where(pa => pa.PatientId == patientId)
                .Select(pa => pa.AllergyId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task SetAllergyIdsForPatientAsync(Guid patientId, IReadOnlyList<string> allergyIds, CancellationToken cancellationToken = default)
        {
            await _context.PatientAllergies
                .Where(pa => pa.PatientId == patientId)
                .ExecuteDeleteAsync(cancellationToken)
                .ConfigureAwait(false);
            foreach (var allergyId in allergyIds)
            {
                _context.PatientAllergies.Add(new PatientAllergy { PatientId = patientId, AllergyId = allergyId });
            }
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task AddClinicalStateAsync(ClinicalState clinicalState, CancellationToken cancellationToken = default)
        {
            _context.ClinicalStates.Add(clinicalState);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task AddClinicalStateAndPublishAsync(ClinicalState clinicalState, CancellationToken cancellationToken = default)
        {
            await ExecuteInTransactionAsync(async ct =>
            {
                await AddClinicalStateAsync(clinicalState, ct).ConfigureAwait(false);
                var response = await _mealsApiClient.PublishClinicalStateAsync(clinicalState.Id, clinicalState.Name, ct).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> UpdateClinicalStateAsync(string id, string name, CancellationToken cancellationToken = default)
        {
            var clinicalState = await _context.ClinicalStates.FirstOrDefaultAsync(c => c.Id == id, cancellationToken).ConfigureAwait(false);
            if (clinicalState is null) return false;
            clinicalState.Name = name;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            var response = await _mealsApiClient.PublishClinicalStateUpdateAsync(id, name, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<ClinicalState?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.ClinicalStates
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<ClinicalState>> ListClinicalStatesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.ClinicalStates
                .OrderBy(c => c.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<string>> GetClinicalStateIdsByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            return await _context.PatientClinicalStates
                .Where(pc => pc.PatientId == patientId)
                .Select(pc => pc.ClinicalStateId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task SetClinicalStateIdsForPatientAsync(Guid patientId, IReadOnlyList<string> clinicalStateIds, CancellationToken cancellationToken = default)
        {
            await _context.PatientClinicalStates
                .Where(pc => pc.PatientId == patientId)
                .ExecuteDeleteAsync(cancellationToken)
                .ConfigureAwait(false);
            foreach (var clinicalStateId in clinicalStateIds)
            {
                _context.PatientClinicalStates.Add(new PatientClinicalState { PatientId = patientId, ClinicalStateId = clinicalStateId });
            }
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task AddDietTypeAsync(DietType dietType, CancellationToken cancellationToken = default)
        {
            _context.DietTypes.Add(dietType);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task AddDietTypeAndPublishAsync(DietType dietType, CancellationToken cancellationToken = default)
        {
            await ExecuteInTransactionAsync(async ct =>
            {
                await AddDietTypeAsync(dietType, ct).ConfigureAwait(false);
                var response = await _mealsApiClient.PublishDietTypeAsync(dietType.Id, dietType.Name, ct).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> UpdateDietTypeAsync(string id, string name, CancellationToken cancellationToken = default)
        {
            var dietType = await _context.DietTypes.FirstOrDefaultAsync(d => d.Id == id, cancellationToken).ConfigureAwait(false);
            if (dietType is null) return false;
            dietType.Name = name;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            var response = await _mealsApiClient.PublishDietTypeUpdateAsync(id, name, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<DietType?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.DietTypes
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<DietType>> ListDietTypesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.DietTypes
                .OrderBy(d => d.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> work, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await work(cancellationToken).ConfigureAwait(false);
                await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }
        }
    }
}
