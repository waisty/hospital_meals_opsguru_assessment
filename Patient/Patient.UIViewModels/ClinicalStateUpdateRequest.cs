namespace Hospital.Patient.ViewModels
{
    public class ClinicalStateUpdateRequest
    {
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
