using Hospital.Patient.ViewModels;

namespace Hospital.Patient.Core.InternalModels;

/// <summary>Used when listing patients with diet type name from a join (no extra queries).</summary>
internal sealed class PatientWithDietTypeName : Patient
{
    public string DietTypeName { get; set; } = "";

    
}
