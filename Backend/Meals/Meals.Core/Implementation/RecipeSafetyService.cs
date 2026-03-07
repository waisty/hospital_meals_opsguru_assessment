using Hospital.Meals.Core.Contracts;
using Hospital.Meals.ViewModels;

namespace Hospital.Meals.Core.Implementation;

internal sealed class RecipeSafetyService : IRecipeSafetyService
{
    private readonly IRecipeRepo _recipeRepo;
    private readonly IIngredientRepo _ingredientRepo;
    private readonly IPatientApiClient _patientApiClient;

    public RecipeSafetyService(
        IRecipeRepo recipeRepo,
        IIngredientRepo ingredientRepo,
        IPatientApiClient patientApiClient)
    {
        _recipeRepo = recipeRepo;
        _ingredientRepo = ingredientRepo;
        _patientApiClient = patientApiClient;
    }

    public async Task<SafetyCheckViewModel> CheckRecipeSafetyAsync(string patientId, string recipeId, CancellationToken cancellationToken = default)
    {
        var patientState = await _patientApiClient.GetPatientDetailAsync(patientId, cancellationToken).ConfigureAwait(false);
        if (patientState == null)
            return new SafetyCheckViewModel { IsSafe = false, StatusReason = "Patient not found", UnsafeIngredientId = null };

        var recipe = await _recipeRepo.GetRecipeByIdAsync(recipeId, cancellationToken).ConfigureAwait(false);
        if (recipe == null)
            return new SafetyCheckViewModel { IsSafe = false, StatusReason = "Recipe not found", UnsafeIngredientId = null };

        var recipeIngredients = await _recipeRepo.GetRecipeIngredientsByRecipeIdAsync(recipeId, cancellationToken).ConfigureAwait(false);
        var allergyIds = patientState.AllergyIds ?? [];
        var clinicalStateIds = patientState.ClinicalStateIds ?? [];
        var dietTypeId = patientState.DietTypeId;

        foreach (var recipeIngredient in recipeIngredients)
        {
            var ingredientId = recipeIngredient.IngredientId;

            if (allergyIds.Count > 0)
            {
                var allergyExclusionIds = await _ingredientRepo.GetAllergyIdsByIngredientIdAsync(ingredientId, cancellationToken).ConfigureAwait(false);
                if (allergyIds.Any(aid => allergyExclusionIds.Contains(aid)))
                    return new SafetyCheckViewModel { IsSafe = false, StatusReason = "Allergen detected", UnsafeIngredientId = ingredientId };
            }

            if (clinicalStateIds.Count > 0)
            {
                var clinicalStateExclusionIds = await _ingredientRepo.GetClinicalStateIdsByIngredientIdAsync(ingredientId, cancellationToken).ConfigureAwait(false);
                if (clinicalStateIds.Any(cid => clinicalStateExclusionIds.Contains(cid)))
                    return new SafetyCheckViewModel { IsSafe = false, StatusReason = "Clinical state contraindication", UnsafeIngredientId = ingredientId };
            }

            if (!string.IsNullOrEmpty(dietTypeId))
            {
                var dietTypeExclusionIds = await _ingredientRepo.GetDietTypeExclusionIdsByIngredientIdAsync(ingredientId, cancellationToken).ConfigureAwait(false);
                if (dietTypeExclusionIds.Contains(dietTypeId))
                    return new SafetyCheckViewModel { IsSafe = false, StatusReason = "Diet type exclusion", UnsafeIngredientId = ingredientId };
            }
        }

        return new SafetyCheckViewModel { IsSafe = true, StatusReason = null, UnsafeIngredientId = null };
    }
}
