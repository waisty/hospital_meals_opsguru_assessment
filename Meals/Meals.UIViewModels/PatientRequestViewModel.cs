namespace Hospital.Meals.ViewModels
{
    public class PatientRequestViewModel
    {
        public Guid Id { get; set; }
        public string PatientId { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string RecipeId { get; set; } = "";
        public DateTime RequestedForDate { get; set; }
        public MealRequestApprovalStatus ApprovalStatus { get; set; }
        public string? StatusReason { get; set; }
    }
}
