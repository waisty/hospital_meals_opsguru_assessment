namespace Hospital.Patient.UIViewModels
{
    public class PatientCreateRequest
    {
        [NonBlank]
        public string Name { get; set; } = "";
        [NonBlank]
        public string MobileNumber { get; set; } = "";
        public string DietTypeId { get; set; } = "";
        public string? Notes { get; set; }
    }
}
