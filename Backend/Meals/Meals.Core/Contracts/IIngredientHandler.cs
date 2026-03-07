using Hospital.Meals.ViewModels;

namespace Hospital.Meals.Core.Contracts
{
    public interface IIngredientHandler
    {
        Task AddIngredientAsync(IngredientCreateRequest request, CancellationToken cancellationToken = default);
        Task<IngredientViewModel?> GetIngredientByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IngredientDetailViewModel?> GetIngredientDetailByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<IngredientViewModel>> ListIngredientsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetAllergyIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default);
        Task SetAllergyExclusionsForIngredientAsync(string ingredientId, SetIngredientAllergyExclusionsRequest request, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetClinicalStateIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default);
        Task SetClinicalStateExclusionsForIngredientAsync(string ingredientId, SetIngredientClinicalStateExclusionsRequest request, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetDietTypeExclusionIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default);
        Task SetDietTypeExclusionsForIngredientAsync(string ingredientId, SetIngredientDietTypeExclusionsRequest request, CancellationToken cancellationToken = default);
    }
}
