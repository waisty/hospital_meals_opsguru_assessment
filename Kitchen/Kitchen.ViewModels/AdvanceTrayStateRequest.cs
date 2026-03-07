namespace Hospital.Kitchen.ViewModels;

/// <summary>
/// Request to advance a tray to the next state.
/// FromState must match the tray's current state (optimistic concurrency).
/// </summary>
public sealed class AdvanceTrayStateRequest
{
    public Guid TrayId { get; set; }
    /// <summary>Current state of the tray (0=Pending, 1=PreparationStarted, 2=AccuracyValidated, 3=EnRoute, 4=Delivered, 5=Retrieved).</summary>
    public int FromState { get; set; }
}
