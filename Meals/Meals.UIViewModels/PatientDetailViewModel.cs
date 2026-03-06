namespace Hospital.Meals.ViewModels;

/// <summary>
/// Patient detail response from the Patient service (GET /api/v1/patients/{id}/detail).
/// </summary>
public class PatientDetailViewModel
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string MobileNumber { get; set; } = "";
    public string DietTypeId { get; set; } = "";
    public string? Notes { get; set; }
    public IReadOnlyList<string> AllergyIds { get; set; } = [];
    public IReadOnlyList<string> ClinicalStateIds { get; set; } = [];
}
