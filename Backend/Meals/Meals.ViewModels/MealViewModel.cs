namespace Hospital.Meals.ViewModels
{
    public class MealViewModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public bool Disabled { get; set; }
        public IReadOnlyList<MealRecipeViewModel> Recipes { get; set; } = Array.Empty<MealRecipeViewModel>();
        /// <summary>Number of recipes linked to this meal (for list views; 0 when not loaded).</summary>
        public int RecipeCount { get; set; }
    }
}
