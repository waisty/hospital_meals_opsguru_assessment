namespace Hospital.Meals.ViewModels
{
    public class RecipeIngredientViewModel
    {
        public string IngredientId { get; set; } = "";
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
        public string IngredientName { get; set; } = "";
    }
}
