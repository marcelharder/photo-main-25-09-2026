namespace api.helpers;
public class GetCategoriesRequest
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public List<int>? cp { get; set; } = new();
    public int Category { get; set; } = 0;
}