namespace Patient.UIViewModels
{
    /// <summary>
    /// Request to set the list of allergy ids for a patient.
    /// </summary>
    public class PatientAllergiesUpdateRequest
    {
        public IReadOnlyList<string> AllergyIds { get; set; } = [];
    }
}
