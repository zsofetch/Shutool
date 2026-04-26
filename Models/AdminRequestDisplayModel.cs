namespace Shutool.Models
{
    public class AdminRequestDisplayModel
    {
        public string ShortId { get; set; } // Just the first few characters of the UUID
        public string TimeFormatted { get; set; }
        public string RequestedBy { get; set; }
        public string HandledBy { get; set; }
        public string Status { get; set; }
    }
}