namespace fotoservice.data.dtos;

public class HolidayDto
{
    public int? id { get; set; }
    public string? description { get; set; }
    public string? title { get; set; }
    public string? coverImageUrl { get; set; }
    public DateTime? startDate { get; set; }
    public DateTime? endDate { get; set; }
    public required locationDto?[] locations { get; set; }
}

public class locationDto
{
    public int? id { get; set; }
    public string? name { get; set; }
    public string? description { get; set; }
    public string? coverImageUrl { get; set; }
    public DateTime? startDate { get; set; }
    public DateTime? endDate { get; set; }
    public required location_ImageDto?[] images { get; set; }
}

public class location_ImageDto
{
    public int? id { get; set; }
    public string? description { get; set; }
    public string? imageUrl { get; set; }
}