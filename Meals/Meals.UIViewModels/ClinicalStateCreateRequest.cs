namespace Hospital.Meals.ViewModels
{
    public class ClinicalStateCreateRequest
    {
        public string Id { get; set; } = "";
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
