using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;

namespace Hospital.Meals.Core.Contracts
{
    internal interface IIngredientRepo
    {
        Task AddIngredientAsync(Ingredient ingredient, CancellationToken cancellationToken = default);
        Task<Ingredient?> GetIngredientByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<Ingredient>> ListIngredientsAsync(int page, int pageSize, string? search = null, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetAllergyIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default);
        Task SetAllergyExclusionsForIngredientAsync(string ingredientId, IReadOnlyList<string> allergyIds, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetClinicalStateIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default);
        Task SetClinicalStateExclusionsForIngredientAsync(string ingredientId, IReadOnlyList<string> clinicalStateIds, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetDietTypeExclusionIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default);
        Task SetDietTypeExclusionsForIngredientAsync(string ingredientId, IReadOnlyList<string> dietTypeIds, CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetAllergyExclusionIdsByIngredientIdsAsync(IEnumerable<string> ingredientIds, CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetClinicalStateExclusionIdsByIngredientIdsAsync(IEnumerable<string> ingredientIds, CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetDietTypeExclusionIdsByIngredientIdsAsync(IEnumerable<string> ingredientIds, CancellationToken cancellationToken = default);
    }
}
