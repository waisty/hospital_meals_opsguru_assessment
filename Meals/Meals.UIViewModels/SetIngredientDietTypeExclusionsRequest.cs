namespace Hospital.Meals.ViewModels
{
    public class SetIngredientDietTypeExclusionsRequest
    {
        public IReadOnlyList<string> DietTypeIds { get; set; } = [];
    }
}
