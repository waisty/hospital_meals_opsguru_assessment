using Hospital.Kitchen.Core.InternalModels;
using static Hospital.Kitchen.Core.Contracts.Enums;

namespace Hospital.Kitchen.Core.Contracts
{
    internal interface IKitchenRepo
    {
        /// <summary>
        /// Adds the tray and its ingredients (tray.TrayIngredients) in a single transaction.
        /// Also records the initial state in tray status history.
        /// </summary>
        Task AddTrayWithIngredientsAsync(Tray tray, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the tray's State and appends a record to tray status history. Returns false if tray not found or current state does not match fromState.
        /// </summary>
        Task<bool> AdvanceTrayStateAsync(Guid trayId, TrayState fromState, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists trays with optional filter by state and/or uncompleted only (state != Retrieved). Results are ordered by ReceivedDateTime descending.
        /// </summary>
        Task<PagedResult<Tray>> ListTraysAsync(int page, int pageSize, TrayState? state, bool uncompletedOnly, CancellationToken cancellationToken = default);
    }
}
