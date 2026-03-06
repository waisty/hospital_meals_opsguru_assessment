namespace Hospital.Meals.UIViewModels
{
    public class PatientMealRequestCreateRequest
    {
        public string PatientId { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string RecipeId { get; set; } = "";
        public DateTime RequestedForDate { get; set; }
    }
}
