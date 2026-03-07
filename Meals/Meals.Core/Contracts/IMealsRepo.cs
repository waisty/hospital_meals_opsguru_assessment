using Hospital.Meals.Core.InternalModels;
using Hospital.Meals.ViewModels;
using static Hospital.Meals.Core.Contracts.Enums;

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
        Task<IReadOnlyList<RecipeIngredientWithName>> GetRecipeIngredientsByRecipeIdAsync(string recipeId, CancellationToken cancellationToken = default);
        Task SetRecipeIngredientsForRecipeAsync(string recipeId, IReadOnlyList<RecipeIngredient> recipeIngredients, CancellationToken cancellationToken = default);

        // Patient request
        //Task AddPatientRequestAsync(PatientRequest request, CancellationToken cancellationToken = default);
        Task UpdatePatientRequestAsync(PatientRequest request, CancellationToken cancellationToken = default);
        /// <summary>
        /// Adds the patient request, fetches patient state, verifies recipe safety (allergies, clinical states, diet type), and updates the request. Returns the new request id.
        /// </summary>
        Task<(Guid requestId, MealRequestAppprovalStatus status, string? statusReason, string unsafeIngredientId)> AddPatientRequestWithSafetyCheckAsync(PatientRequestCreateRequest request, CancellationToken cancellationToken);
        Task<PatientRequest?> GetPatientRequestByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<PatientRequest>> ListPatientRequestsAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        // Allergy (reference data)
        Task AddAllergyAsync(Allergy allergy, CancellationToken cancellationToken = default);
        Task<bool> UpdateAllergyAsync(string id, string name, CancellationToken cancellationToken = default);
        Task<Allergy?> GetAllergyByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Allergy>> ListAllergiesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetAllergyIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default);
        Task SetAllergyExclusionsForIngredientAsync(string ingredientId, IReadOnlyList<string> allergyIds, CancellationToken cancellationToken = default);

        // Clinical state (reference data)
        Task AddClinicalStateAsync(ClinicalState clinicalState, CancellationToken cancellationToken = default);
        Task<bool> UpdateClinicalStateAsync(string id, string name, CancellationToken cancellationToken = default);
        Task<ClinicalState?> GetClinicalStateByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ClinicalState>> ListClinicalStatesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetClinicalStateIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default);
        Task SetClinicalStateExclusionsForIngredientAsync(string ingredientId, IReadOnlyList<string> clinicalStateIds, CancellationToken cancellationToken = default);

        // Diet type (reference data)
        Task AddDietTypeAsync(DietType dietType, CancellationToken cancellationToken = default);
        Task<bool> UpdateDietTypeAsync(string id, string name, CancellationToken cancellationToken = default);
        Task<DietType?> GetDietTypeByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DietType>> ListDietTypesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetDietTypeExclusionIdsByIngredientIdAsync(string ingredientId, CancellationToken cancellationToken = default);
        Task SetDietTypeExclusionsForIngredientAsync(string ingredientId, IReadOnlyList<string> dietTypeIds, CancellationToken cancellationToken = default);
    }
}
