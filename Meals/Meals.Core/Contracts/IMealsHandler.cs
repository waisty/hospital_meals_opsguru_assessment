using Hospital.Meals.UIViewModels;

namespace Hospital.Meals.Core.Contracts
{
    public interface IMealsHandler
    {
        // Meal
        Task AddMealAsync(MealCreateRequest request, CancellationToken cancellationToken = default);
        Task<MealViewModel?> GetMealByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<MealViewModel>> ListMealsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        // Recipe
        Task AddRecipeAsync(RecipeCreateRequest request, CancellationToken cancellationToken = default);
        Task<RecipeViewModel?> GetRecipeByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<RecipeDetailViewModel?> GetRecipeDetailByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<RecipeViewModel>> ListRecipesAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        // Ingredient
        Task AddIngredientAsync(IngredientCreateRequest request, CancellationToken cancellationToken = default);
        Task<IngredientViewModel?> GetIngredientByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IngredientDetailViewModel?> GetIngredientDetailByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<IngredientViewModel>> ListIngredientsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        // Recipe ingredients
        Task<IReadOnlyList<RecipeIngredientViewModel>> GetRecipeIngredientsByRecipeIdAsync(string recipeId, CancellationToken cancellationToken = default);
        Task SetRecipeIngredientsAsync(string recipeId, SetRecipeIngredientsRequest request, CancellationToken cancellationToken = default);

        // Patient meal request
        Task<Guid> AddPatientMealRequestAsync(PatientMealRequestCreateRequest request, CancellationToken cancellationToken = default);
        Task<PatientMealRequestViewModel?> GetPatientMealRequestByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<PatientMealRequestViewModel>> ListPatientMealRequestsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        // Allergy (reference data)
        Task AddAllergyAsync(AllergyCreateRequest request, CancellationToken cancellationToken = default);
        Task<AllergyViewModel?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AllergyViewModel>> ListAllergiesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetAllergyIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default);
        Task SetAllergyExclusionsForIngredientAsync(string ingredientId, SetIngredientAllergyExclusionsRequest request, CancellationToken cancellationToken = default);

        // Clinical state (reference data)
        Task AddClinicalStateAsync(ClinicalStateCreateRequest request, CancellationToken cancellationToken = default);
        Task<ClinicalStateViewModel?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ClinicalStateViewModel>> ListClinicalStatesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetClinicalStateIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default);
        Task SetClinicalStateExclusionsForIngredientAsync(string ingredientId, SetIngredientClinicalStateExclusionsRequest request, CancellationToken cancellationToken = default);

        // Diet type (reference data)
        Task AddDietTypeAsync(DietTypeCreateRequest request, CancellationToken cancellationToken = default);
        Task<DietTypeViewModel?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DietTypeViewModel>> ListDietTypesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetDietTypeExclusionIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default);
        Task SetDietTypeExclusionsForIngredientAsync(string ingredientId, SetIngredientDietTypeExclusionsRequest request, CancellationToken cancellationToken = default);
    }
}
