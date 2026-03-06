namespace Hospital.Meals.UIViewModels
{
    public class IngredientDetailViewModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public IReadOnlyList<string> AllergyExclusionIds { get; set; } = [];
        public IReadOnlyList<string> ClinicalStateExclusionIds { get; set; } = [];
        public IReadOnlyList<string> DietTypeExclusionIds { get; set; } = [];
    }
}
