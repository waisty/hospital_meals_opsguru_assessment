namespace Hospital.Meals.ViewModels;

public class IngredientExclusionNamesItem
{
    public string IngredientId { get; set; } = "";
    public IReadOnlyList<string> AllergyNames { get; set; } = [];
    public IReadOnlyList<string> ClinicalStateNames { get; set; } = [];
    public IReadOnlyList<string> DietTypeNames { get; set; } = [];
}
