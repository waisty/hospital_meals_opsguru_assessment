namespace Hospital.Patient.ViewModels
{
    public class PatientCreateRequest
    {
        [NonBlank]
        public string FirstName { get; set; } = "";
        public string MiddleName { get; set; } = "";
        [NonBlank]
        public string LastName { get; set; } = "";
        [NonBlank]
        public string MobileNumber { get; set; } = "";
        public string DietTypeId { get; set; } = "";
        public string? Notes { get; set; }
    }
}
