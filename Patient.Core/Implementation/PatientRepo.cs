using Microsoft.EntityFrameworkCore;
using Hospital.Contracts;
using Hospital.Patient.Core.Contracts;
using Hospital.Patient.Core.InternalModels;

namespace Hospital.Patient.Core.Implementation
{
    internal sealed class PatientRepo : IPatientRepo
    {
        private readonly PatientDBContext _context;

        public PatientRepo(PatientDBContext context)
        {
            _context = context;
        }

        public async Task AddPatientAsync(InternalModels.Patient patient, CancellationToken cancellationToken = default)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<InternalModels.Patient?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Patients
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<PagedResult<InternalModels.Patient>> ListPatientsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await _context.Patients.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await _context.Patients
                .AsNoTracking()
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

        public async Task<Allergy?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Allergies
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<Allergy>> ListAllergiesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Allergies
                .AsNoTracking()
                .OrderBy(a => a.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<string>> GetAllergyIdsByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            return await _context.PatientAllergies
                .AsNoTracking()
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

        public async Task<ClinicalState?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.ClinicalStates
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<ClinicalState>> ListClinicalStatesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.ClinicalStates
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<string>> GetClinicalStateIdsByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            return await _context.PatientClinicalStates
                .AsNoTracking()
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

        public async Task<DietType?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.DietTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<DietType>> ListDietTypesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.DietTypes
                .AsNoTracking()
                .OrderBy(d => d.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
