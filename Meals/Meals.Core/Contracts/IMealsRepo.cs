using Hospital.Meals.Core.InternalModels;

namespace Hospital.Meals.Core.Contracts
{
    internal interface IMealsRepo
    {
        // Meal
        Task AddMealAsync(Meal meal, CancellationToken cancellationToken = default);
        Task<Meal?> GetMealByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<Meal>> ListMealsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        // Recipe
        Task AddRecipeAsync(Recipe recipe, CancellationToken cancellationToken = default);
        Task<Recipe?> GetRecipeByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<Recipe>> ListRecipesAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        // Ingredient
        Task AddIngredientAsync(Ingredient ingredient, CancellationToken cancellationToken = default);
        Task<Ingredient?> GetIngredientByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<PagedResult<Ingredient>> ListIngredientsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        // Recipe ingredient
        Task<IReadOnlyList<RecipeIngredient>> GetRecipeIngredientsByRecipeIdAsync(string recipeId, CancellationToken cancellationToken = default);
        Task SetRecipeIngredientsForRecipeAsync(string recipeId, IReadOnlyList<RecipeIngredient> recipeIngredients, CancellationToken cancellationToken = default);

        // Patient meal request
        Task AddPatientMealRequestAsync(PatientMealRequest request, CancellationToken cancellationToken = default);
        Task<PatientMealRequest?> GetPatientMealRequestByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<PatientMealRequest>> ListPatientMealRequestsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        // Allergy (reference data)
        Task AddAllergyAsync(Allergy allergy, CancellationToken cancellationToken = default);
        Task<bool> UpdateAllergyAsync(string id, string name, CancellationToken cancellationToken = default);
        Task<Allergy?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Allergy>> ListAllergiesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetAllergyIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default);
        Task SetAllergyExclusionsForIngredientAsync(string ingredientId, IReadOnlyList<string> allergyIds, CancellationToken cancellationToken = default);

        // Clinical state (reference data)
        Task AddClinicalStateAsync(ClinicalState clinicalState, CancellationToken cancellationToken = default);
        Task<ClinicalState?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ClinicalState>> ListClinicalStatesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetClinicalStateIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default);
        Task SetClinicalStateExclusionsForIngredientAsync(string ingredientId, IReadOnlyList<string> clinicalStateIds, CancellationToken cancellationToken = default);

        // Diet type (reference data)
        Task AddDietTypeAsync(DietType dietType, CancellationToken cancellationToken = default);
        Task<DietType?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DietType>> ListDietTypesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetDietTypeExclusionIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default);
        Task SetDietTypeExclusionsForIngredientAsync(string ingredientId, IReadOnlyList<string> dietTypeIds, CancellationToken cancellationToken = default);
    }
}
