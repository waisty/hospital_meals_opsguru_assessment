namespace Hospital.Meals.ViewModels
{
    public class SetIngredientAllergyExclusionsRequest
    {
        public IReadOnlyList<string> AllergyIds { get; set; } = [];
    }
}
