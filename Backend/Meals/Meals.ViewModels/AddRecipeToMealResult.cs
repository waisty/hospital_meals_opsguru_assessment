namespace Hospital.Meals.ViewModels;

/// <summary>
/// Result of adding a recipe to a meal. When Success is false, ExistingMealName is set when the recipe is already mapped to another meal.
/// </summary>
public class AddRecipeToMealResult
{
    public bool Success { get; set; }
    public string? ExistingMealName { get; set; }
}
