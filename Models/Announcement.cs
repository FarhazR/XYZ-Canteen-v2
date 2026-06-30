// Models/Announcement.cs
public class Announcement
{
    public int AnnouncementID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string PublishedBy { get; set; } = string.Empty; // from JWT claims
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}