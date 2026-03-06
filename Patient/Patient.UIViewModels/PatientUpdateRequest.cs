namespace Hospital.Patient.UIViewModels
{
    public class PatientUpdateRequest
    {
        public string Name { get; set; } = "";
        public string MobileNumber { get; set; } = "";
        public string DietTypeId { get; set; } = "";
        public string? Notes { get; set; }
    }
}
