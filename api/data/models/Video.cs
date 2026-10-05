namespace fotoservice.data.models;

public class Video
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required TimeSpan? Duration { get; set; }
    public required string ThumbnailPath {get; set;}

}