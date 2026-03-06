namespace Hospital.Patient.UIViewModels
{
    public class DietTypeUpdateRequest
    {
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
