namespace Hospital.Meals.ViewModels
{
    public class AllergyCreateRequest
    {
        public string Id { get; set; } = "";
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
