using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;

namespace Hospital.Meals.Core.Contracts
{
    internal interface IRecipeRepo
    {
        Task AddRecipeAsync(Recipe recipe, CancellationToken cancellationToken = default);
        Task<bool> UpdateRecipeAsync(string id, string name, string? description, CancellationToken cancellationToken = default);
        Task<Recipe?> GetRecipeByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<RecipeWithMealName>> ListRecipesAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RecipeIngredientWithName>> GetRecipeIngredientsByRecipeIdAsync(string recipeId, CancellationToken cancellationToken = default);
        Task SetRecipeIngredientsForRecipeAsync(string recipeId, IReadOnlyList<RecipeIngredient> recipeIngredients, CancellationToken cancellationToken = default);
    }
}
