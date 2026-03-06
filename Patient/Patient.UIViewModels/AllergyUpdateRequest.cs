namespace Hospital.Patient.UIViewModels
{
    public class AllergyUpdateRequest
    {
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
