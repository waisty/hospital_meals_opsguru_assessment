namespace Hospital.Meals.ViewModels;

/// <summary>
/// Result of checking whether a recipe is safe for a patient (allergies, clinical states, diet type).
/// </summary>
public class SafetyCheckViewModel
{
    public bool IsSafe { get; set; }
    public string? StatusReason { get; set; }
    public string? UnsafeIngredientId { get; set; }
}
