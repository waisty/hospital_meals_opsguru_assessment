using static Hospital.Kitchen.Core.Contracts.Enums;

namespace Hospital.Kitchen.ServiceViewModels
{
    public sealed class TrayIngredientItem
    {
        public string IngredientName { get; set; } = "";
        public decimal Qty { get; set; }
        public string? Unit { get; set; }
    }

    public sealed class CreateTrayRequest
    {
        public Guid PatientMealRequestId { get; set; }
        public string PatientId { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string MiddleName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string RecipeName { get; set; } = "";
        public TrayState State { get; set; } = TrayState.Pending;
        public IReadOnlyList<TrayIngredientItem> Ingredients { get; set; } = [];
    }
}
