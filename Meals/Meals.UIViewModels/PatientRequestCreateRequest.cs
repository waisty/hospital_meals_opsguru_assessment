namespace Hospital.Meals.ViewModels
{
    public class PatientRequestCreateRequest
    {
        public string PatientId { get; set; } = "";
        //[NonBlank]
        //public string PatientName { get; set; } = "";
        public string RecipeId { get; set; } = "";
    }
}
