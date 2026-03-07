using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;

namespace Hospital.Meals.Core.Contracts
{
    internal interface IMealRepo
    {
        Task AddMealAsync(Meal meal, CancellationToken cancellationToken = default);
        Task<Meal?> GetMealByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MealRecipe>> GetMealRecipesByMealIdAsync(string mealId, CancellationToken cancellationToken = default);
        Task<PagedResult<Meal>> ListMealsAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<string, int>> GetRecipeCountByMealIdsAsync(IEnumerable<string> mealIds, CancellationToken cancellationToken = default);
        Task<bool> UpdateMealAsync(string id, string name, string? description, CancellationToken cancellationToken = default);
        Task<bool> AddRecipeToMealAsync(string mealId, string recipeId, CancellationToken cancellationToken = default);
        Task<bool> SetMealRecipeDisabledAsync(string mealId, string recipeId, bool disabled, CancellationToken cancellationToken = default);
    }
}
