namespace Hospital.Meals.ViewModels
{
    public class DietTypeCreateRequest
    {
        public string Id { get; set; } = "";
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
