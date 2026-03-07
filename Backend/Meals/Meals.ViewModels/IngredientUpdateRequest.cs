namespace Hospital.Meals.ViewModels
{
    public class IngredientUpdateRequest
    {
        [NonBlank]
        public string Name { get; set; } = "";
        public string? Description { get; set; }
    }
}
