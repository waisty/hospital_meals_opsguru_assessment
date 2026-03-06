namespace Hospital.Meals.ViewModels
{
    public class MealViewModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string RecipeId { get; set; } = "";
        public string? DietTypeId { get; set; }
        public bool Disabled { get; set; }
    }
}
