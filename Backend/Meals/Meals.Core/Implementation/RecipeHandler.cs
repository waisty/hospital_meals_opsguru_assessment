using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;

namespace Hospital.Meals.Core.Implementation
{
    internal sealed class RecipeHandler : IRecipeHandler
    {
        private readonly IRecipeRepo _repo;

        public RecipeHandler(IRecipeRepo repo)
        {
            _repo = repo;
        }

        public async Task AddRecipeAsync(RecipeCreateRequest request, CancellationToken cancellationToken = default)
        {
            var recipe = new Recipe
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description
            };
            await _repo.AddRecipeAsync(recipe, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> UpdateRecipeAsync(string id, RecipeUpdateRequest request, CancellationToken cancellationToken = default)
        {
            return await _repo.UpdateRecipeAsync(id, request.Name, request.Description, cancellationToken).ConfigureAwait(false);
        }

        public async Task<RecipeViewModel?> GetRecipeByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var recipe = await _repo.GetRecipeByIdAsync(id, cancellationToken).ConfigureAwait(false);
            return recipe == null ? null : recipe.ToRecipeViewModel();
        }

        public async Task<RecipeDetailViewModel?> GetRecipeDetailByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var recipe = await _repo.GetRecipeByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (recipe == null) return null;

            var ingredients = await _repo.GetRecipeIngredientsByRecipeIdAsync(id, cancellationToken).ConfigureAwait(false);
            return recipe.ToRecipeDetailViewModel(ingredients);
        }

        public async Task<PagedResult<RecipeViewModel>> ListRecipesAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default)
        {
            var paged = await _repo.ListRecipesAsync(page, pageSize, search, cancellationToken).ConfigureAwait(false);
            return new PagedResult<RecipeViewModel>
            {
                Items = paged.Items.Select(r => r.ToRecipeViewModel()).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<IReadOnlyList<RecipeIngredientViewModel>> GetRecipeIngredientsByRecipeIdAsync(string recipeId, CancellationToken cancellationToken = default)
        {
            var list = await _repo.GetRecipeIngredientsByRecipeIdAsync(recipeId, cancellationToken).ConfigureAwait(false);
            return list.Select(ri => ri.ToRecipeIngredientViewModel()).ToList();
        }

        public async Task SetRecipeIngredientsAsync(string recipeId, SetRecipeIngredientsRequest request, CancellationToken cancellationToken = default)
        {
            var recipeIngredients = (request.Ingredients ?? [])
                .Select(i => new RecipeIngredient
                {
                    RecipeId = recipeId,
                    IngredientId = i.IngredientId,
                    Quantity = i.Quantity,
                    Unit = i.Unit
                })
                .ToList();
            await _repo.SetRecipeIngredientsForRecipeAsync(recipeId, recipeIngredients, cancellationToken).ConfigureAwait(false);
        }
    }
}
