namespace Hospital.Meals.ViewModels
{
    public class AllergyUpdateRequest
    {
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
