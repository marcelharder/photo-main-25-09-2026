namespace fotoservice.api.helpers;

public class ComSettings
{
    public string? VideoUrl { get; set; }

    public string? PhotoContainerUrl { get; set; }

    public string? HolidayContainerUrl { get; set; }

    public HttpContextAccessor? HttpContextAccessor { get; set; }


}