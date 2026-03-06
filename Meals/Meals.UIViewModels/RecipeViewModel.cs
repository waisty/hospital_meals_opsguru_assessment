namespace Hospital.Meals.ViewModels
{
    public class RecipeViewModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? DietTypeId { get; set; }
        public bool Disabled { get; set; }
    }
}
