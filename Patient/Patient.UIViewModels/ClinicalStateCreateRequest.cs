namespace Hospital.Patient.UIViewModels
{
    public class ClinicalStateCreateRequest
    {
        //public string Id { get; set; } = "";
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
