namespace Hospital.Kitchen.Core.Contracts
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
        public string PatientName { get; set; } = "";
        public string RecipeName { get; set; } = "";
        public Enums.TrayState State { get; set; } = Enums.TrayState.Pending;
        public IReadOnlyList<TrayIngredientItem> Ingredients { get; set; } = [];
    }
}
