namespace Hospital.Meals.ViewModels
{
    public class MealUpdateRequest
    {
        [NonBlank]
        public string Name { get; set; } = "";
        public string? Description { get; set; }
    }
}
