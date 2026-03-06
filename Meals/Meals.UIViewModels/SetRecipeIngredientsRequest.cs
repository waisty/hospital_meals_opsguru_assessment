namespace Hospital.Meals.ViewModels
{
    public class SetRecipeIngredientsRequest
    {
        public IReadOnlyList<RecipeIngredientViewModel> Ingredients { get; set; } = [];
    }
}
