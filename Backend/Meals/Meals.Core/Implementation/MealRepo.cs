using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Meals.Core.Implementation
{
    internal sealed class MealRepo : IMealRepo
    {
        private readonly MealsDBContext _context;

        public MealRepo(MealsDBContext context)
        {
            _context = context;
        }

        public async Task AddMealAsync(Meal meal, CancellationToken cancellationToken = default)
        {
            _context.Meals.Add(meal);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<Meal?> GetMealByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Meals
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<PagedResult<Meal>> ListMealsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var totalCount = await _context.Meals.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await _context.Meals
                .OrderBy(m => m.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return new PagedResult<Meal>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
