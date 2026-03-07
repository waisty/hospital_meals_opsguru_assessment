using Hospital.Meals.ViewModels;

namespace Hospital.Meals.Core.Contracts
{
    public interface IRecipeHandler
    {
        Task AddRecipeAsync(RecipeCreateRequest request, CancellationToken cancellationToken = default);
        Task<bool> UpdateRecipeAsync(string id, RecipeUpdateRequest request, CancellationToken cancellationToken = default);
        Task<RecipeViewModel?> GetRecipeByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<RecipeDetailViewModel?> GetRecipeDetailByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<RecipeViewModel>> ListRecipesAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default);
        Task<RecipeExclusionNamesResponseViewModel> GetExclusionNamesByRecipeIdsAsync(RecipeExclusionNamesRequest request, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RecipeIngredientViewModel>> GetRecipeIngredientsByRecipeIdAsync(string recipeId, CancellationToken cancellationToken = default);
        Task SetRecipeIngredientsAsync(string recipeId, SetRecipeIngredientsRequest request, CancellationToken cancellationToken = default);
    }
}
