namespace Hospital.Meals.UIViewModels
{
    public class DietTypeCreateRequest
    {
        public string Id { get; set; } = "";
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
