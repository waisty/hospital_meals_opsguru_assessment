namespace Hospital.Patient.ViewModels
{
    /// <summary>
    /// Patient with associated allergy and clinical state ids for display or edit.
    /// </summary>
    public class PatientDetailViewModel
    {
        public string Id { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string MiddleName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string MobileNumber { get; set; } = "";
        public string DietTypeId { get; set; } = "";
        public string? Notes { get; set; }
        public IReadOnlyList<PatientAllergyViewModel> Allergies { get; set; } = [];
        public IReadOnlyList<PatientClinicalStateViewModel> ClinicalStates { get; set; } = [];
    }
}
