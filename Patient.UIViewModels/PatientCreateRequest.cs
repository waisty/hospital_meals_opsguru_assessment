namespace Patient.UIViewModels
{
    public class PatientCreateRequest
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string DietTypeId { get; set; } = "";
        public string? Notes { get; set; }
    }
}
