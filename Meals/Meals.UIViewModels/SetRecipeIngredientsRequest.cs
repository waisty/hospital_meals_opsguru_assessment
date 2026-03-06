namespace Hospital.Meals.UIViewModels
{
    public class SetRecipeIngredientsRequest
    {
        public IReadOnlyList<RecipeIngredientViewModel> Ingredients { get; set; } = [];
    }
}
