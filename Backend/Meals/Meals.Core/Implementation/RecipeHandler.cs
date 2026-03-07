using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;

namespace Hospital.Meals.Core.Implementation
{
    internal sealed class RecipeHandler : IRecipeHandler
    {
        private readonly IRecipeRepo _repo;
        private readonly IMealRepo _mealRepo;
        private readonly IIngredientHandler _ingredientHandler;

        public RecipeHandler(IRecipeRepo repo, IMealRepo mealRepo, IIngredientHandler ingredientHandler)
        {
            _repo = repo;
            _mealRepo = mealRepo;
            _ingredientHandler = ingredientHandler;
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
            var mappedMeal = await _mealRepo.GetMealByRecipeIdAsync(id, cancellationToken).ConfigureAwait(false);
            var detail = recipe.ToRecipeDetailViewModel(ingredients, mappedMeal?.Name);

            var ingredientIds = ingredients.Select(i => i.IngredientId).Distinct().ToList();
            if (ingredientIds.Count > 0)
            {
                var exclusionResponse = await _ingredientHandler.GetExclusionNamesByIngredientIdsAsync(
                    new IngredientExclusionNamesRequest { IngredientIds = ingredientIds },
                    cancellationToken).ConfigureAwait(false);
                detail.ExclusionNamesByIngredientId = exclusionResponse.Items.ToDictionary(x => x.IngredientId);
            }

            return detail;
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

        public async Task<RecipeExclusionNamesResponseViewModel> GetExclusionNamesByRecipeIdsAsync(RecipeExclusionNamesRequest request, CancellationToken cancellationToken = default)
        {
            var recipeIds = (request.RecipeIds ?? []).Distinct().ToList();
            if (recipeIds.Count == 0)
                return new RecipeExclusionNamesResponseViewModel { Items = [] };

            var ingredientIdsByRecipe = await _repo.GetIngredientIdsByRecipeIdsAsync(recipeIds, cancellationToken).ConfigureAwait(false);
            var allIngredientIds = ingredientIdsByRecipe.Values.SelectMany(ids => ids).Distinct().ToList();

            var exclusionNamesByIngredient = allIngredientIds.Count > 0
                ? await _ingredientHandler.GetExclusionNamesByIngredientIdsAsync(
                    new IngredientExclusionNamesRequest { IngredientIds = allIngredientIds },
                    cancellationToken).ConfigureAwait(false)
                : new IngredientExclusionNamesResponse { Items = [] };

            var exclusionByIngredient = exclusionNamesByIngredient.Items.ToDictionary(x => x.IngredientId);

            var items = recipeIds.Select(recipeId =>
            {
                var allergyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var clinicalStateNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var dietTypeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (ingredientIdsByRecipe.TryGetValue(recipeId, out var ingIds))
                {
                    foreach (var ingId in ingIds)
                    {
                        if (exclusionByIngredient.TryGetValue(ingId, out var ex))
                        {
                            foreach (var n in ex.AllergyNames) allergyNames.Add(n);
                            foreach (var n in ex.ClinicalStateNames) clinicalStateNames.Add(n);
                            foreach (var n in ex.DietTypeNames) dietTypeNames.Add(n);
                        }
                    }
                }
                return new RecipeExclusionNamesItemViewModel
                {
                    RecipeId = recipeId,
                    AllergyNames = allergyNames.ToList(),
                    ClinicalStateNames = clinicalStateNames.ToList(),
                    DietTypeNames = dietTypeNames.ToList()
                };
            }).ToList();

            return new RecipeExclusionNamesResponseViewModel { Items = items };
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
