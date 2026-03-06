namespace Hospital.Meals.UIViewModels
{
    public class IngredientCreateRequest
    {
        public string Id { get; set; } = "";
        [NonBlank]
        public string Name { get; set; } = "";
        public string? Description { get; set; }
    }
}
