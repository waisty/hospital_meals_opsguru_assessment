namespace Hospital.Patient.ViewModels
{
    public class PatientViewModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string MobileNumber { get; set; } = "";
        public string DietTypeId { get; set; } = "";
        public string? Notes { get; set; }
    }
}
