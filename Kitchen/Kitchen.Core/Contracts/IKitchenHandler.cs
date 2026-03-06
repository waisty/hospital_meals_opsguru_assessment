namespace Hospital.Kitchen.Core.Contracts
{
    public interface IKitchenHandler
    {
        /// <summary>
        /// Creates a tray and its ingredients in a single transaction.
        /// </summary>
        Task<Guid> CreateTrayAsync(CreateTrayRequest request, CancellationToken cancellationToken = default);
    }
}
