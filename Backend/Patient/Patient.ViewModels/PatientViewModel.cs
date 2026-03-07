namespace Hospital.Patient.ViewModels
{
    public class PatientViewModel
    {
        public string Id { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string MiddleName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string MobileNumber { get; set; } = "";
        public string DietTypeId { get; set; } = "";
        /// <summary>Diet type name (from join when listing patients).</summary>
        public string? Notes { get; set; }
    }

    public class PatientWithDietTypeNameViewModel : PatientViewModel
    {
        public string DietTypeName { get; set; } = "";
    }
}
