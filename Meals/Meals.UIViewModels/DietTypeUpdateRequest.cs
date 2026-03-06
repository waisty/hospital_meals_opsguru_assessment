namespace Hospital.Meals.ViewModels
{
    public class DietTypeUpdateRequest
    {
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
