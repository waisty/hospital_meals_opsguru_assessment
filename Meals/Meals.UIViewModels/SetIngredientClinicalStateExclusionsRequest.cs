namespace Hospital.Meals.UIViewModels
{
    public class SetIngredientClinicalStateExclusionsRequest
    {
        public IReadOnlyList<string> ClinicalStateIds { get; set; } = [];
    }
}
