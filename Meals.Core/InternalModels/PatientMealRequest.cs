using System;
using static Hospital.Meals.Core.Contracts.Enums;

namespace Hospital.Meals.Core.InternalModels
{
    internal class PatientMealRequest
    {
        public Guid Id { get; set; }
        public string PatientId { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string RecipeId { get; set; } = "";
        public DateTime RequestedForDate { get; set; }
        public MealRequestAppprovalStatus ApprovalStatus { get; set; }
        public string? StatusReason { get; set; }
    }
}
