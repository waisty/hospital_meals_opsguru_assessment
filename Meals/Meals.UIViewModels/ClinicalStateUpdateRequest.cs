namespace Hospital.Meals.UIViewModels
{
    public class ClinicalStateUpdateRequest
    {
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
