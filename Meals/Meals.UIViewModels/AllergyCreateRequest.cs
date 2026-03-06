namespace Hospital.Meals.UIViewModels
{
    public class AllergyCreateRequest
    {
        public string Id { get; set; } = "";
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
