using Hospital.Meals.ViewModels;

namespace Hospital.Meals.Core.Contracts;

/// <summary>
/// HTTP client for outbound calls from the Meals service to the Kitchen API.
/// </summary>
public interface IKitchenApiClient
{
    /// <summary>
    /// Publishes a tray to the Kitchen service (POST /api/v1/trays).
    /// Returns the created tray id.
    /// </summary>
    Task<Guid> PublishTrayAsync(KitchenPublishTrayRequest request, CancellationToken cancellationToken = default);
}
