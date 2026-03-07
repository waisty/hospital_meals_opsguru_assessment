namespace Hospital.Patient.ServiceViewModels
{
    /// <summary>
    /// Lightweight patient detail for inter-service communication (Meals service).
    /// Returns allergy/clinical-state IDs only, without names.
    /// </summary>
    public class PatientServiceDetailViewModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string MobileNumber { get; set; } = "";
        public string DietTypeId { get; set; } = "";
        public string? Notes { get; set; }
        public IReadOnlyList<string> AllergyIds { get; set; } = [];
        public IReadOnlyList<string> ClinicalStateIds { get; set; } = [];
    }
}
