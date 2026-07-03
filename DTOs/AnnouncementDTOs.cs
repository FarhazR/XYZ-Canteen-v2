namespace CanteenAPI.DTOs
{
    public class CreateAnnouncementDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class AnnouncementResponseDto
    {
        public int AnnouncementID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string PublishedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}