

namespace api.data.implementations;

using Microsoft.Extensions.Options;
using System.Net.Http.Json;

public class HolidayService : IHolidayService
{
    private readonly ComSettings _comSettings;
    private readonly HttpClient _httpClient;
    private readonly ILogger<HolidayService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HolidayService(
        HttpClient httpClient,
        IOptions<ComSettings> comSettings,
        ILogger<HolidayService> logger)
    {
        _httpClient = httpClient;
        _comSettings = comSettings.Value;
        _logger = logger;
    }

    public async Task<string> CreateHoliday(Holiday up, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(up);

        using var response = await _httpClient.PostAsJsonAsync(
            $"{GetBaseUrl()}/api/holidays",
            up,
            cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task DeleteHoliday(int id)
    {
        using var response = await _httpClient.DeleteAsync(
            $"{GetBaseUrl()}/api/holidays/{id}",
            CancellationToken.None);
        response.EnsureSuccessStatusCode();
    }

    public async Task<HolidayPageDto?> GetAllHolidays(
        HolidaysParams cp,
        CancellationToken cancellationToken = default)
    {
        ValidatePaging(cp.PageNumber, cp.PageSize);
        if (string.IsNullOrWhiteSpace(_comSettings.HolidayContainerUrl))
            throw new InvalidOperationException("HolidayContainerUrl is not configured.");
      
        var url = $"{_comSettings.HolidayContainerUrl.TrimEnd('/')}/api/holidays/paged?page={cp.PageNumber}&pageSize={cp.PageSize}";


        
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var root = document.RootElement;

        if (root.ValueKind == JsonValueKind.Array)
        {
            var items = root.Deserialize<HolidayDto[]>(JsonOptions)
                ?? throw new InvalidOperationException($"The holiday service returned an invalid list from {url}.");

            return CreateHolidayPage(items, cp.PageNumber, cp.PageSize, items.Length);
        }

        if (root.ValueKind != JsonValueKind.Object ||
            !TryGetPropertyIgnoreCase(root, "items", out var itemsElement) ||
            itemsElement.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidOperationException(
                $"The holiday service returned an unsupported paged response from {url}; expected an object with an items array.");
        }

        var pageItems = itemsElement.Deserialize<HolidayDto[]>(JsonOptions)
            ?? throw new InvalidOperationException($"The holiday service returned an invalid items array from {url}.");
        var page = GetInt32Property(root, "page") ?? cp.PageNumber;
        var pageSize = GetInt32Property(root, "pageSize") ?? cp.PageSize;
        var totalCount = GetInt32Property(root, "totalCount") ?? pageItems.Length;
        var totalPages = GetInt32Property(root, "totalPages")
            ?? (int)Math.Ceiling(totalCount / (double)pageSize);

        return new HolidayPageDto
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = pageItems.ToList()
        };
    }

    public async Task<HolidayDto[]> GetAllHolidaysForAdmin(CancellationToken cancellationToken = default)
    {
        var url = $"{GetBaseUrl()}/api/holidays";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<HolidayDto[]>(
            stream,
            JsonOptions,
            cancellationToken);

        return result
            ?? throw new InvalidOperationException($"The holiday service returned an empty response from {url}.");
    }

    public async Task<PagedList<HolidayDto>?> GetAllowedHolidays(
        HolidaysParams cp,
        CancellationToken cancellationToken = default)
    {
        ValidatePaging(cp.PageNumber, cp.PageSize);
        var url = $"{GetBaseUrl()}/api/holidays/allowed";
        var request = new
        {
            cp.PageNumber,
            cp.PageSize,
            cp.userId,
            cp.Category,
            cp.AllowedHolidays
        };

        using var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<HolidayDto[]>(
            stream,
            JsonOptions,
            cancellationToken);

        if (result == null)
        {
            _logger.LogWarning("Null response from {Url} for user {UserId}", url, cp.userId);
            return null;
        }
        return PagedList<HolidayDto>.CreateAsync(result, cp.PageNumber, cp.PageSize)
            ?? throw new InvalidOperationException("Failed to create the paged allowed-holiday list.");
    }

    public async Task<string> GetHolidayDescription(int id, CancellationToken cancellationToken = default)
    {
        var url = $"{GetBaseUrl()}/api/holidays/{id}/description";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

   public async Task<FileStreamResult> GetImage(
    int id,
    CancellationToken cancellationToken = default)
{

    var url = $"{_comSettings.HolidayContainerUrl.TrimEnd('/')}/api/location-images/{id}";

    //var url = $"{GetBaseUrl()}/api/location-images/{id}";

    var response = await _httpClient.GetAsync(
        url,
        HttpCompletionOption.ResponseHeadersRead,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

    var contentType =
        response.Content.Headers.ContentType?.MediaType
        ?? "application/octet-stream";

    return new FileStreamResult(stream, contentType);
}

    public async Task<HolidayDto> GetSpecificHoliday(int id, CancellationToken cancellationToken = default)
    {
        var url = $"{GetBaseUrl()}/api/holidays/{id}";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        return await JsonSerializer.DeserializeAsync<HolidayDto>(stream, JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException($"The holiday service returned an empty response for holiday {id}.");
    }

    public async Task<int> getTotalNumberOfHolidaysForUser(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var url = $"{GetBaseUrl()}/api/holidays/users/{userId}/count";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<int>(stream, JsonOptions, cancellationToken);
    }

    public async Task<string> UpdateHoliday(Holiday up, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(up);

        using var response = await _httpClient.PutAsJsonAsync(
            $"{GetBaseUrl()}/api/holidays/{up.Id}",
            up,
            cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    private string GetBaseUrl()
    {
        if (string.IsNullOrWhiteSpace(_comSettings.HolidayContainerUrl))
            throw new InvalidOperationException("HolidayContainerUrl is not configured.");

        return _comSettings.HolidayContainerUrl.TrimEnd('/');
    }

    private static void ValidatePaging(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be at least 1.");

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be at least 1.");
    }

    private static HolidayPageDto CreateHolidayPage(
        IEnumerable<HolidayDto> items,
        int page,
        int pageSize,
        int totalCount)
    {
        return new HolidayPageDto
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            Items = items.ToList()
        };
    }

    private static bool TryGetPropertyIgnoreCase(
        JsonElement element,
        string propertyName,
        out JsonElement value)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    private static int? GetInt32Property(JsonElement element, string propertyName)
    {
        return TryGetPropertyIgnoreCase(element, propertyName, out var value) &&
            value.ValueKind == JsonValueKind.Number &&
            value.TryGetInt32(out var result)
                ? result
                : null;
    }

   
}
