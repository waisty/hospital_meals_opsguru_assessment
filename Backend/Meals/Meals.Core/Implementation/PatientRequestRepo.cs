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

        public async Task<PagedResult<PatientRequest>> ListPatientRequestsAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default)
        {
            // Full-text search only runs when search is at least 2 characters (same as patients table)
            if (!string.IsNullOrWhiteSpace(search) && search.Trim().Length < 2)
                search = null;

            var query = _context.PatientRequests.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = string.Join(" & ", search!
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => $"{word}:*"));

                var matchingIds = await _context.Database
                    .SqlQueryRaw<Guid>(
                        "SELECT id FROM dbo.patient_requests WHERE search_vector @@ to_tsquery('simple', {0})",
                        term)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
                if (matchingIds.Count == 0)
                {
                    return new PagedResult<PatientRequest>
                    {
                        Items = new List<PatientRequest>(),
                        TotalCount = 0,
                        Page = page,
                        PageSize = pageSize
                    };
                }
                query = query.Where(r => matchingIds.Contains(r.Id));
            }

            query = query.OrderBy(r => r.RequestedDateTime).ThenBy(r => r.PatientId);

            var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await query
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
