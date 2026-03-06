namespace Hospital.Meals.UIViewModels
{
    public class DietTypeUpdateRequest
    {
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
