namespace Hospital.Patient.ViewModels;

public class BatchPatientAllergiesRequest
{
    public IList<string> PatientIds { get; set; } = new List<string>();
}
