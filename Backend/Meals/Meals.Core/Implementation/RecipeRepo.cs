using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Meals.Core.Implementation
{
    internal sealed class RecipeRepo : IRecipeRepo
    {
        private readonly MealsDBContext _context;

        public RecipeRepo(MealsDBContext context)
        {
            _context = context;
        }

        public async Task AddRecipeAsync(Recipe recipe, CancellationToken cancellationToken = default)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> UpdateRecipeAsync(string id, string name, string? description, CancellationToken cancellationToken = default)
        {
            var recipe = await _context.Recipes.FirstOrDefaultAsync(r => r.Id == id, cancellationToken).ConfigureAwait(false);
            if (recipe == null) return false;
            recipe.Name = name;
            recipe.Description = description;
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }

        public async Task<Recipe?> GetRecipeByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Recipes
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<PagedResult<RecipeWithMealName>> ListRecipesAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default)
        {
            // Full-text search only runs when search is at least 2 characters (same as ingredients)
            if (!string.IsNullOrWhiteSpace(search) && search.Trim().Length < 2)
                search = null;

            var baseQuery = _context.Recipes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = string.Join(" & ", search!
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => $"{word}:*"));

                var matchingIds = await _context.Database
                    .SqlQueryRaw<string>(
                        "SELECT id FROM dbo.recipes WHERE search_vector @@ to_tsquery('simple', {0})",
                        term)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
                if (matchingIds.Count == 0)
                {
                    return new PagedResult<RecipeWithMealName>
                    {
                        Items = new List<RecipeWithMealName>(),
                        TotalCount = 0,
                        Page = page,
                        PageSize = pageSize
                    };
                }
                baseQuery = baseQuery.Where(r => matchingIds.Contains(r.Id));
            }

            var recipeToMealName = from mr in _context.MealRecipes
                                  join m in _context.Meals on mr.MealId equals m.Id
                                  select new { mr.RecipeId, m.Name };
            var queryWithMeal = from r in baseQuery
                               join mn in recipeToMealName on r.Id equals mn.RecipeId into mnGroup
                               from mn in mnGroup.DefaultIfEmpty()
                               select new RecipeWithMealName
                               {
                                   Id = r.Id,
                                   Name = r.Name,
                                   Description = r.Description,
                                   Disabled = r.Disabled,
                                   MealName = mn != null ? mn.Name : null
                               };

            queryWithMeal = queryWithMeal.OrderBy(x => x.Name);

            var totalCount = await queryWithMeal.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await queryWithMeal
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return new PagedResult<RecipeWithMealName>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetIngredientIdsByRecipeIdsAsync(IEnumerable<string> recipeIds, CancellationToken cancellationToken = default)
        {
            var idList = recipeIds.Distinct().ToList();
            if (idList.Count == 0)
                return new Dictionary<string, IReadOnlyList<string>>();

            var pairs = await _context.RecipeIngredients
                .Where(ri => idList.Contains(ri.RecipeId))
                .Select(ri => new { ri.RecipeId, ri.IngredientId })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return pairs
                .GroupBy(x => x.RecipeId)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<string>)g.Select(x => x.IngredientId).Distinct().ToList());
        }

        public async Task<IReadOnlyList<RecipeIngredientWithName>> GetRecipeIngredientsByRecipeIdAsync(string recipeId, CancellationToken cancellationToken = default)
        {
            var ret = await (from recipeIngredient in _context.RecipeIngredients
                              where recipeIngredient.RecipeId == recipeId
                              join ingredient in _context.Ingredients on recipeIngredient.IngredientId equals ingredient.Id
                              orderby ingredient.Name
                              select new { recipeIngredient, ingredientName = ingredient.Name }).ToAsyncEnumerable().Select(x =>
                              {
                                  return x.recipeIngredient.ToRecipeIngredientWithName(x.ingredientName);
                              }).ToListAsync().ConfigureAwait(false);

            return ret;
        }

        public async Task SetRecipeIngredientsForRecipeAsync(string recipeId, IReadOnlyList<RecipeIngredient> recipeIngredients, CancellationToken cancellationToken = default)
        {
            await _context.RecipeIngredients
                .Where(ri => ri.RecipeId == recipeId)
                .ExecuteDeleteAsync(cancellationToken)
                .ConfigureAwait(false);
            foreach (var ri in recipeIngredients)
            {
                _context.RecipeIngredients.Add(new RecipeIngredient
                {
                    RecipeId = recipeId,
                    IngredientId = ri.IngredientId,
                    Quantity = ri.Quantity,
                    Unit = ri.Unit
                });
            }
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
