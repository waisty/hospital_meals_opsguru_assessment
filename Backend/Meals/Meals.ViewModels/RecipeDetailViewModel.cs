namespace Hospital.Meals.ViewModels
{
    public class RecipeDetailViewModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public bool Disabled { get; set; }
        /// <summary>Name of the meal this recipe is mapped to, if any. A recipe can only be mapped to one meal.</summary>
        public string? MappedMealName { get; set; }
        public IReadOnlyList<RecipeIngredientViewModel> Ingredients { get; set; } = [];
        /// <summary>Ingredient ID to exclusion names for that ingredient (for request summary and reusable display).</summary>
        public IReadOnlyDictionary<string, IngredientExclusionNamesItem>? ExclusionNamesByIngredientId { get; set; }
    }
}
