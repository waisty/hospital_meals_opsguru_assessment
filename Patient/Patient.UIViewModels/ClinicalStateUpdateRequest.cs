namespace Hospital.Patient.UIViewModels
{
    public class ClinicalStateUpdateRequest
    {
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
