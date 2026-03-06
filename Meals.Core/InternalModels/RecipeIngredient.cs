namespace Hospital.Meals.Core.InternalModels
{
    internal class RecipeIngredient
    {
        public string RecipeId { get; set; } = "";
        public string IngredientId { get; set; } = "";
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
    }
}
