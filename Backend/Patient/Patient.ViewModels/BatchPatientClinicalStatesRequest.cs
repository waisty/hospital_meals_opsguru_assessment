namespace Hospital.Patient.ViewModels;

public class BatchPatientClinicalStatesRequest
{
    public IList<string> PatientIds { get; set; } = new List<string>();
}
