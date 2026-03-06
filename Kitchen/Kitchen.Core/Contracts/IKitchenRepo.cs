using Hospital.Kitchen.Core.InternalModels;

namespace Hospital.Kitchen.Core.Contracts
{
    internal interface IKitchenRepo
    {
        /// <summary>
        /// Adds the tray and its ingredients (tray.TrayIngredients) in a single transaction.
        /// </summary>
        Task AddTrayWithIngredientsAsync(Tray tray, CancellationToken cancellationToken = default);
    }
}
