namespace Patient.UIViewModels
{
    /// <summary>
    /// Represents a single page of items with total count for pagination (UI/API layer).
    /// </summary>
    public class PagedResultViewModel<T>
    {
        public IReadOnlyList<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
