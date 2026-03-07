namespace Hospital.Patient.ViewModels;

public class BatchPatientAllergiesResponse
{
    public IList<PatientAllergiesItemViewModel> Items { get; set; } = new List<PatientAllergiesItemViewModel>();
}

public class PatientAllergiesItemViewModel
{
    public string PatientId { get; set; } = "";
    public IList<string> AllergyNames { get; set; } = new List<string>();
}
