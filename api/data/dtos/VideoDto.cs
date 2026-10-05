namespace api.Helpers;

public class VideoDto
{
    public int Id { get; set; }
    public string Title { get; set; } = String.Empty;
    public string Category { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public string Duration { get; set; } = String.Empty;
    public string FilePath { get; set; } = String.Empty;
    public string ContentType { get; set; } = String.Empty;
    public string ThumbnailPath { get; set; } = String.Empty;
}
