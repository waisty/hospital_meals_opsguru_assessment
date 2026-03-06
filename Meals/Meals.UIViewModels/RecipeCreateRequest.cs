namespace Hospital.Meals.UIViewModels
{
    public class RecipeCreateRequest
    {
        public string Id { get; set; } = "";
        [NonBlank]
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? DietTypeId { get; set; }
    }
}
