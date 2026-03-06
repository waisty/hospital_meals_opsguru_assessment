namespace Hospital.Meals.UIViewModels
{
    public class SetIngredientAllergyExclusionsRequest
    {
        public IReadOnlyList<string> AllergyIds { get; set; } = [];
    }
}
