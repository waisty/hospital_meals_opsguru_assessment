namespace Hospital.Meals.ViewModels
{
    public class RecipeViewModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public bool Disabled { get; set; }
        /// <summary>Name of the meal this recipe is mapped to, if any.</summary>
        public string? MappedMealName { get; set; }
    }
}
