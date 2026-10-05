namespace api.data.implementations;

using Microsoft.Build.Framework;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Threading;

public class CategoryService : ICategoryService
{

    private readonly ComSettings _comSettings;
    private readonly HttpClient _httpClient;
    private readonly UserManager<AppUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
       HttpClient httpClient,
        UserManager<AppUser> userManager,
        IOptions<ComSettings> comSettings,
        IHttpContextAccessor httpContextAccessor,
        ILogger<CategoryService> logger)
    {
        _httpClient = httpClient;
        _comSettings = comSettings.Value;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<int> getTotalNumberOfCategoriesForUser(
    int userId,
    CancellationToken cancellationToken = default)
    {
        if (_comSettings.PhotoContainerUrl == null)
            throw new InvalidOperationException("PhotoContainerUrl is not configured.");

        var url = $"{_comSettings.PhotoContainerUrl.TrimEnd('/')}/Category/getTotalNumberOfCategoriesForUser/{userId}";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        var result = await JsonSerializer.DeserializeAsync<int>(
            stream,
            _jsonOptions,
            cancellationToken);
        result = 145;

        return result;
    }

    public async Task<PagedList<CategoryDto>?> GetAllowedCategories(CategoryParams cp, CancellationToken cancellationToken = default)
    {
        if (_comSettings.PhotoContainerUrl == null)
            throw new InvalidOperationException("PhotoContainerUrl is not configured.");

        var url = $"{_comSettings.PhotoContainerUrl.TrimEnd('/')}/Category/getAllowedCategories";

        var query = new GetCategoriesRequest
        {
            PageNumber = cp.PageNumber,
            PageSize = cp.PageSize,
            cp = cp.AllowedCategories
        };

        using var response = await _httpClient.PostAsync(url, JsonContent.Create(query), cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<CategoryDto[]>(
            stream,
            _jsonOptions,
            cancellationToken);

        if (result == null)
        {
            _logger.LogWarning("Null response from {Url} for Page {Page}", url, cp.PageNumber);
            return null;
        }

        return PagedList<CategoryDto>.CreateAsync(result.AsQueryable(), cp.PageNumber, cp.PageSize);
    }

    public async Task<string> GetCategoryDescription(int id, CancellationToken cancellationToken = default)
    {
        if (_comSettings.PhotoContainerUrl == null)
            throw new InvalidOperationException("PhotoContainerUrl is not configured.");

        var url = $"{_comSettings.PhotoContainerUrl.TrimEnd('/')}/Category/getDescription/{id}";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

  

    public async Task<string> UpdateCategory(Category up, CancellationToken cancellationToken = default)
    {
        if (_comSettings.PhotoContainerUrl == null)
            throw new InvalidOperationException("PhotoContainerUrl is not configured.");
        
        var url = $"{_comSettings.PhotoContainerUrl.TrimEnd('/')}/Category/updateCategory";
        using var response = await _httpClient.PutAsJsonAsync(url, up, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task<string> CreateCategory(Category up, CancellationToken cancellationToken = default)
    {
         if (_comSettings.PhotoContainerUrl == null)
            throw new InvalidOperationException("PhotoContainerUrl is not configured.");
        
        var url = $"{_comSettings.PhotoContainerUrl.TrimEnd('/')}/Category/addCategory";
        using var response = await _httpClient.PostAsJsonAsync(url, up, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

      /*
    these methods are not yet available in the photocontainer, so i will implement them later when they are available.
    */

    public Task DeleteCategory(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<CategoryDto> GetSpecificCategory(int id, CancellationToken cancellationToken = default)
    {
        if (_comSettings.PhotoContainerUrl == null)
            throw new InvalidOperationException("PhotoContainerUrl is not configured.");

        var url = $"{_comSettings.PhotoContainerUrl.TrimEnd('/')}/Category/getSpecificCategory/{id}";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrEmpty(result))
        {
            _logger.LogWarning("Null or empty response from {Url} for Category ID {Id}", url, id);
            throw new InvalidOperationException($"Response content for category {id} was empty.");
        }

        return JsonSerializer.Deserialize<CategoryDto>(result, _jsonOptions)
            ?? throw new InvalidOperationException($"Failed to deserialize category {id}.");
    }



    public async Task<PagedList<CategoryDto>> GetAllCategories(
    CategoryParams cp,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_comSettings.PhotoContainerUrl))
            throw new InvalidOperationException("PhotoContainerUrl is not configured.");

        var url = $"{_comSettings.PhotoContainerUrl.TrimEnd('/')}/Category/getAllCategories";

        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var content = await response.Content.ReadAsStreamAsync(cancellationToken);

        var result = await JsonSerializer.DeserializeAsync<List<CategoryDto>>(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            },
            cancellationToken
        ) ?? new List<CategoryDto>();

        var pagedResult = PagedList<CategoryDto>.CreateAsync(result, cp.PageNumber, cp.PageSize);
        return pagedResult ?? throw new InvalidOperationException("Failed to create paged category list.");
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<CategoryDto[]> GetAllCategoriesForAdmin(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_comSettings.PhotoContainerUrl))
            throw new InvalidOperationException("PhotoContainerUrl is not configured.");

        var url = $"{_comSettings.PhotoContainerUrl.TrimEnd('/')}/Category/getAllCategories";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var content = await response.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<CategoryDto[]>(
            content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            },
            cancellationToken
        ) ?? Array.Empty<CategoryDto>();
        return result;

    }

   
}
