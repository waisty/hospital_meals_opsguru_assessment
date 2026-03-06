namespace Hospital.Patient.ViewModels
{
    public class AllergyUpdateRequest
    {
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
