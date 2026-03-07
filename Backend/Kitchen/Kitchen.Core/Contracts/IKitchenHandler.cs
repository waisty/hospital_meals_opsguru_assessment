using Hospital.Kitchen.ServiceViewModels;
using Hospital.Kitchen.ViewModels;

namespace Hospital.Kitchen.Core.Contracts
{
    public interface IKitchenHandler
    {
        /// <summary>
        /// Creates a tray and its ingredients in a single transaction.
        /// </summary>
        Task<Guid> CreateTrayAsync(CreateTrayRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Advances the tray to the next state. Returns false if tray not found, current state does not match fromState, or already at final state (Retrieved).
        /// </summary>
        Task<bool> AdvanceTrayStateAsync(Guid trayId, Enums.TrayState fromState, CancellationToken cancellationToken = default);
    }
}
