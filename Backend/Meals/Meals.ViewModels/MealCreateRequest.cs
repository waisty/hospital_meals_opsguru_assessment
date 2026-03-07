namespace Hospital.Meals.ViewModels
{
    public class MealCreateRequest
    {
        public string Id { get; set; } = "";
        [NonBlank]
        public string Name { get; set; } = "";
        public string? Description { get; set; }
    }
}
