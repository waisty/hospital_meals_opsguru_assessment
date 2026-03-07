namespace Hospital.Patient.ViewModels;

public class BatchPatientClinicalStatesResponse
{
    public IList<PatientClinicalStatesItemViewModel> Items { get; set; } = new List<PatientClinicalStatesItemViewModel>();
}

public class PatientClinicalStatesItemViewModel
{
    public string PatientId { get; set; } = "";
    public IList<string> ClinicalStateNames { get; set; } = new List<string>();
}
