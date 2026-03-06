namespace Hospital.Meals.ViewModels
{
    public class SetIngredientClinicalStateExclusionsRequest
    {
        public IReadOnlyList<string> ClinicalStateIds { get; set; } = [];
    }
}
