namespace Patient.UIViewModels
{
    /// <summary>
    /// Request to set the list of clinical state ids for a patient.
    /// </summary>
    public class PatientClinicalStatesUpdateRequest
    {
        public IReadOnlyList<string> ClinicalStateIds { get; set; } = [];
    }
}
