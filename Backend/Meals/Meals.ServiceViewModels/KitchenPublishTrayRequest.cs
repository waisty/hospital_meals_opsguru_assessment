namespace Hospital.Meals.ServiceViewModels;

/// <summary>
/// Request shape for publishing a tray to the Kitchen API (POST /api/v1/trays).
/// Matches the Kitchen service CreateTrayRequest contract.
/// </summary>
public sealed class KitchenPublishTrayRequest
{
    public Guid PatientMealRequestId { get; set; }
    public string PatientId { get; set; } = "";
    public string PatientName { get; set; } = "";
    public string RecipeName { get; set; } = "";
    public IReadOnlyList<KitchenPublishTrayIngredientItem> Ingredients { get; set; } = [];
}

public sealed class KitchenPublishTrayIngredientItem
{
    public string IngredientName { get; set; } = "";
    public decimal Qty { get; set; }
    public string? Unit { get; set; }
}
