using Hospital.Meals.Core.Contracts;
using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;
using MealEntity = Hospital.Meals.Core.InternalModels.Meal;

namespace Hospital.Meals.Core.Implementation
{
    internal sealed class MealHandler : IMealHandler
    {
        private readonly IMealRepo _repo;
        private readonly IRecipeRepo _recipeRepo;

        public MealHandler(IMealRepo repo, IRecipeRepo recipeRepo)
        {
            _repo = repo;
            _recipeRepo = recipeRepo;
        }

        public async Task AddMealAsync(MealCreateRequest request, CancellationToken cancellationToken = default)
        {
            var meal = new MealEntity
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description
            };
            await _repo.AddMealAsync(meal, cancellationToken).ConfigureAwait(false);
        }

        public async Task<MealViewModel?> GetMealByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var meal = await _repo.GetMealByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (meal == null) return null;
            var mealRecipes = await _repo.GetMealRecipesByMealIdAsync(id, cancellationToken).ConfigureAwait(false);
            var recipeIds = mealRecipes.Select(mr => mr.RecipeId).Distinct().ToList();
            var recipeNames = new Dictionary<string, string>();
            foreach (var recipeId in recipeIds)
            {
                var recipe = await _recipeRepo.GetRecipeByIdAsync(recipeId, cancellationToken).ConfigureAwait(false);
                if (recipe != null)
                    recipeNames[recipeId] = recipe.Name;
            }
            return meal.ToMealViewModel(mealRecipes, recipeNames);
        }

        public async Task<PagedResult<MealViewModel>> ListMealsAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default)
        {
            var paged = await _repo.ListMealsAsync(page, pageSize, search, cancellationToken).ConfigureAwait(false);
            var mealIds = paged.Items.Select(m => m.Id).ToList();
            var counts = await _repo.GetRecipeCountByMealIdsAsync(mealIds, cancellationToken).ConfigureAwait(false);
            return new PagedResult<MealViewModel>
            {
                Items = paged.Items.Select(m => m.ToMealViewModelWithRecipeCount(counts.GetValueOrDefault(m.Id, 0))).ToList(),
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize
            };
        }

        public async Task<bool> UpdateMealAsync(string id, MealUpdateRequest request, CancellationToken cancellationToken = default)
        {
            return await _repo.UpdateMealAsync(id, request.Name, request.Description, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<MealRecipeViewModel>> GetMealRecipesAsync(string mealId, CancellationToken cancellationToken = default)
        {
            var mealRecipes = await _repo.GetMealRecipesByMealIdAsync(mealId, cancellationToken).ConfigureAwait(false);
            var result = new List<MealRecipeViewModel>();
            foreach (var mr in mealRecipes)
            {
                var recipe = await _recipeRepo.GetRecipeByIdAsync(mr.RecipeId, cancellationToken).ConfigureAwait(false);
                result.Add(new MealRecipeViewModel
                {
                    RecipeId = mr.RecipeId,
                    RecipeName = recipe?.Name ?? mr.RecipeId,
                    Disabled = mr.Disabled
                });
            }
            return result;
        }

        public async Task<AddRecipeToMealResult> AddRecipeToMealAsync(string mealId, string recipeId, CancellationToken cancellationToken = default)
        {
            var existingMeal = await _repo.GetMealByRecipeIdAsync(recipeId, cancellationToken).ConfigureAwait(false);
            if (existingMeal != null)
            {
                if (existingMeal.Id == mealId)
                    return new AddRecipeToMealResult { Success = false, ExistingMealName = null };
                return new AddRecipeToMealResult { Success = false, ExistingMealName = existingMeal.Name };
            }
            var added = await _repo.AddRecipeToMealAsync(mealId, recipeId, cancellationToken).ConfigureAwait(false);
            return new AddRecipeToMealResult { Success = added, ExistingMealName = null };
        }

        public async Task<bool> SetMealRecipeDisabledAsync(string mealId, string recipeId, bool disabled, CancellationToken cancellationToken = default)
        {
            return await _repo.SetMealRecipeDisabledAsync(mealId, recipeId, disabled, cancellationToken).ConfigureAwait(false);
        }
    }
}
