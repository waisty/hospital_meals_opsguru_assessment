using Hospital.Kitchen.Core.InternalModels;

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
        /// Updates the tray's State and appends a record to tray status history. Returns false if tray not found.
        /// </summary>
        Task<bool> UpdateTrayStateAsync(Guid trayId, string newState, CancellationToken cancellationToken = default);
    }
}
