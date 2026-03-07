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

        public async Task<PagedResult<Recipe>> ListRecipesAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default)
        {
            // Full-text search only runs when search is at least 2 characters (same as ingredients)
            if (!string.IsNullOrWhiteSpace(search) && search.Trim().Length < 2)
                search = null;

            var query = _context.Recipes.AsQueryable();

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
                    return new PagedResult<Recipe>
                    {
                        Items = new List<Recipe>(),
                        TotalCount = 0,
                        Page = page,
                        PageSize = pageSize
                    };
                }
                query = query.Where(r => matchingIds.Contains(r.Id));
            }

            query = query.OrderBy(r => r.Name);

            var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return new PagedResult<Recipe>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
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
