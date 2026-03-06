namespace Hospital.Patient.ViewModels
{
    public class DietTypeUpdateRequest
    {
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
