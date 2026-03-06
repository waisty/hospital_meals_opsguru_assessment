namespace Hospital.Meals.UIViewModels
{
    public class AllergyUpdateRequest
    {
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
