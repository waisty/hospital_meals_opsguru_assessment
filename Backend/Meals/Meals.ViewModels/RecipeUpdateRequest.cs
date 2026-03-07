namespace Hospital.Meals.ViewModels
{
    public class RecipeUpdateRequest
    {
        [NonBlank]
        public string Name { get; set; } = "";
        public string? Description { get; set; }
    }
}
