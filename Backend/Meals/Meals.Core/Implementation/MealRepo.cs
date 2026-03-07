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

        public async Task<PagedResult<Meal>> ListMealsAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(search) && search.Trim().Length < 2)
                search = null;

            var query = _context.Meals.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = string.Join(" & ", search!
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => $"{word}:*"));

                var matchingIds = await _context.Database
                    .SqlQueryRaw<string>(
                        "SELECT id FROM dbo.meals WHERE search_vector @@ to_tsquery('simple', {0})",
                        term)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
                if (matchingIds.Count == 0)
                {
                    return new PagedResult<Meal>
                    {
                        Items = new List<Meal>(),
                        TotalCount = 0,
                        Page = page,
                        PageSize = pageSize
                    };
                }
                query = query.Where(m => matchingIds.Contains(m.Id));
            }

            query = query.OrderBy(m => m.Name);

            var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await query
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

        public async Task<bool> UpdateMealAsync(string id, string name, string recipeId, CancellationToken cancellationToken = default)
        {
            var meal = await _context.Meals.FirstOrDefaultAsync(m => m.Id == id, cancellationToken).ConfigureAwait(false);
            if (meal == null) return false;
            meal.Name = name;
            meal.RecipeId = recipeId;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }
    }
}
