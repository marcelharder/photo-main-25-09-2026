using System.Text.Json.Serialization;

namespace fotoservice.data.dtos;

public class HolidayPageDto
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("items")]
    public List<HolidayDto> Items { get; set; } = new();
}
