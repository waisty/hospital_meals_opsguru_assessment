namespace Patient.Core.Contracts
{
    /// <summary>
    /// Represents a single page of items with total count for pagination.
    /// </summary>
    internal sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];
        public int TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
    }
}
