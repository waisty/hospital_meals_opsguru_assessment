namespace Hospital.Meals.ViewModels
{
    public class ClinicalStateUpdateRequest
    {
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
