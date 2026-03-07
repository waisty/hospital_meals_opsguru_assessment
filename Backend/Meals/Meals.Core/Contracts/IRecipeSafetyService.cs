using Hospital.Meals.ViewModels;

namespace Hospital.Meals.Core.Contracts;

/// <summary>
/// Reusable service that checks whether a recipe is safe for a patient
/// (allergies, clinical states, diet type exclusions). Does not persist any state.
/// </summary>
public interface IRecipeSafetyService
{
    /// <summary>
    /// Checks if the given recipe is safe for the patient. Fetches patient from Patient API
    /// and validates all recipe ingredients against allergy, clinical state, and diet type exclusions.
    /// </summary>
    Task<SafetyCheckViewModel> CheckRecipeSafetyAsync(string patientId, string recipeId, CancellationToken cancellationToken = default);
}
