namespace Hospital.Meals.Core.InternalModels
{
    /// <summary>Recipe list item with joined meal name (one recipe maps to at most one meal).</summary>
    internal class RecipeWithMealName
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public bool Disabled { get; set; }
        public string? MealName { get; set; }
    }
}
