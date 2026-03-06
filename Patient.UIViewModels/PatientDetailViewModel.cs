namespace Hospital.Patient.UIViewModels
{
    /// <summary>
    /// Patient with associated allergy and clinical state ids for display or edit.
    /// </summary>
    public class PatientDetailViewModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string DietTypeId { get; set; } = "";
        public string? Notes { get; set; }
        public IReadOnlyList<string> AllergyIds { get; set; } = [];
        public IReadOnlyList<string> ClinicalStateIds { get; set; } = [];
    }
}
