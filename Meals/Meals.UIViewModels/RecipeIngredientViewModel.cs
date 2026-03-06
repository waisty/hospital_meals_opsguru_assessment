namespace Hospital.Meals.UIViewModels
{
    public class RecipeIngredientViewModel
    {
        public string IngredientId { get; set; } = "";
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
    }
}
