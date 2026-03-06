namespace Hospital.Patient.UIViewModels
{
    public class AllergyCreateRequest
    {
        //public string Id { get; set; } = "";
        [NonBlank]
        public string Name { get; set; } = "";
    }
}
