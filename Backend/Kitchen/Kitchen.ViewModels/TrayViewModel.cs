namespace Hospital.Kitchen.ViewModels
{
    /// <summary>
    /// Tray list item for paginated tray listing.
    /// </summary>
    public sealed class TrayViewModel
    {
        public Guid Id { get; set; }
        public Guid PatientMealRequestId { get; set; }
        public string PatientId { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string RecipeName { get; set; } = "";
        /// <summary>Current state as integer (see Enums.TrayState).</summary>
        public int State { get; set; }
        public DateTime ReceivedDateTime { get; set; }
        public DateTime? LastUpdateDateTime { get; set; }
    }
}
