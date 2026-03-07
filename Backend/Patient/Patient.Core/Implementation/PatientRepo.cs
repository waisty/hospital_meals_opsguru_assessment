using Microsoft.EntityFrameworkCore;
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
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<PagedResult<InternalModels.PatientWithDietTypeName>> ListPatientsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await _context.Patients.CountAsync(cancellationToken).ConfigureAwait(false);
            var query = from p in _context.Patients
                        join dt in _context.DietTypes on p.DietTypeId equals dt.Id
                        orderby p.Name
                        select new { p, dietTypeName = dt.Name };
            var list = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            var items = list.Select(x => x.p.ToPatientWithDietTypeName(x.dietTypeName)).ToList();
            return new PagedResult<InternalModels.PatientWithDietTypeName>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IReadOnlyList<PatientAllergyWithName>> GetPatientAllergiesWithNameAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            var ret = await (from pa in _context.PatientAllergies
                            where pa.PatientId == patientId
                            join allergy in _context.Allergies on pa.AllergyId equals allergy.Id
                            orderby allergy.Name
                            select new { pa, allergyName = allergy.Name }).ToAsyncEnumerable().Select(x =>
                            {
                                return x.pa.ToPatientAllergyWithName(x.allergyName);
                            }).ToListAsync().ConfigureAwait(false);

            return ret;
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

        public async Task<IReadOnlyList<PatientClinicalStateWithName>> GetPatientClinicalStatesWithNameAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            var ret = await (from pc in _context.PatientClinicalStates
                            where pc.PatientId == patientId
                            join cs in _context.ClinicalStates on pc.ClinicalStateId equals cs.Id
                            orderby cs.Name
                            select new { pc, clinicalStateName = cs.Name }).ToAsyncEnumerable().Select(x =>
                            {
                                return x.pc.ToPatientClinicalStateWithName(x.clinicalStateName);
                            }).ToListAsync().ConfigureAwait(false);

            return ret;
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
    }
}
