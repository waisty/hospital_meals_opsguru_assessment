using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Meals.Core.Implementation
{
    internal sealed class PatientRequestRepo : IPatientRequestRepo
    {
        private readonly MealsDBContext _context;

        public PatientRequestRepo(MealsDBContext context)
        {
            _context = context;
        }

        public async Task AddPatientRequestAsync(PatientRequest request, CancellationToken cancellationToken = default)
        {
            _context.PatientRequests.Add(request);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdatePatientRequestAsync(PatientRequest request, CancellationToken cancellationToken = default)
        {
            _context.PatientRequests.Update(request);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<PatientRequest?> GetPatientRequestByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.PatientRequests
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<PagedResult<PatientRequest>> ListPatientRequestsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await _context.PatientRequests.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await _context.PatientRequests
                .OrderBy(r => r.RequestedDateTime)
                .ThenBy(r => r.PatientId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return new PagedResult<PatientRequest>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
