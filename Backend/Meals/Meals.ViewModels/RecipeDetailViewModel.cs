namespace Hospital.Meals.ViewModels
{
    public class RecipeDetailViewModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public bool Disabled { get; set; }
        public IReadOnlyList<RecipeIngredientViewModel> Ingredients { get; set; } = [];
    }
}
