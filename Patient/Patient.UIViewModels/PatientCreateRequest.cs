namespace Hospital.Patient.UIViewModels
{
    public class PatientCreateRequest
    {
        public string Name { get; set; } = "";
        public string DietTypeId { get; set; } = "";
        public string? Notes { get; set; }
    }
}
