namespace Hospital.Meals.ViewModels;

/// <summary>
/// Aggregated exclusion names for a single recipe (distinct across all its ingredients).
/// </summary>
public class RecipeExclusionNamesItemViewModel
{
    public string RecipeId { get; set; } = "";
    public IReadOnlyList<string> AllergyNames { get; set; } = [];
    public IReadOnlyList<string> ClinicalStateNames { get; set; } = [];
    public IReadOnlyList<string> DietTypeNames { get; set; } = [];
}
