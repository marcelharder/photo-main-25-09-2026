namespace api.data.implementations;

using api.data.interfaces;
using api.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

public class VideoService : IVideoService
{
    private DapperContext _context;
    private IConfiguration _conf;
    private readonly HttpClient _httpClient;
    private readonly ComSettings _comSettings;
    private readonly UserManager<AppUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<VideoService> _logger;

    public VideoService(
        UserManager<AppUser> userManager,
        IConfiguration conf,
        DapperContext context,
        HttpClient httpClient,
        IOptions<ComSettings> comSettings,
        IHttpContextAccessor httpContextAccessor,
        ILogger<VideoService> logger)
    {
        _httpClient = httpClient;
        _comSettings = comSettings.Value;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _conf = conf;
        _context = context;
        _userManager = userManager;
    }

    public async Task<string> GetAllVideosAsync(
    VideoParams videoParams,
    CancellationToken cancellationToken)
    {
        var query = QueryString.Create(
            new Dictionary<string, string?>
            {
                ["pageNumber"] = videoParams.PageNumber.ToString(),
                ["pageSize"] = videoParams.PageSize.ToString(),
                ["category"] = videoParams.Category.ToString()
            });
        if (_comSettings.VideoUrl == null)
        {
            throw new InvalidOperationException("VideoUrl is not configured.");
        }
        else
        {
            var url = $"{_comSettings.VideoUrl.TrimEnd('/')}/videoList{query}";

            try
            {
                _logger.LogInformation("Calling video service URL: {Url}", url);

                using var response = await _httpClient.PostAsync(url, null, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogInformation(
                    "Video service responded. StatusCode: {StatusCode}, Body: {Body}",
                    (int)response.StatusCode,
                    content);

                response.EnsureSuccessStatusCode();
                return content;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex,
                    "HTTP error calling video service. StatusCode: {StatusCode}, Message: {Message}, Inner: {Inner}",
                    ex.StatusCode,
                    ex.Message,
                    ex.InnerException?.Message);

                throw;
            }


        }
    }
    public async Task<(Stream Stream, string ContentType)> GetVideoStreamAsync(
    int id,
    CancellationToken cancellationToken
)
    {
        if (_comSettings.VideoUrl == null)
        {
            throw new InvalidOperationException("VideoUrl is not configured.");
        }
        else
        {
            var url = $"{_comSettings.VideoUrl.TrimEnd('/')}/Videos/{id}/stream";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && httpContext.Request.Headers.TryGetValue("Range", out var rangeHeader))
            {
                request.Headers.Add("Range", rangeHeader.ToString());
            }

            var response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                response.Dispose();

                throw new HttpRequestException(
                    string.IsNullOrWhiteSpace(error) ? "Error fetching requested video." : error,
                    null,
                    response.StatusCode
                );
            }

            if (httpContext != null)
            {
                httpContext.Response.StatusCode = (int)response.StatusCode;
                if (response.Content.Headers.ContentLength.HasValue)
                    httpContext.Response.Headers.ContentLength = response.Content.Headers.ContentLength;
                if (response.Content.Headers.TryGetValues("Content-Range", out var contentRange))
                    httpContext.Response.Headers["Content-Range"] = contentRange.ToArray();
                if (response.Headers.AcceptRanges.Count > 0)
                    httpContext.Response.Headers["Accept-Ranges"] = "bytes";
            }

            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var contentType =
                response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";

            return (stream, contentType);
        }
    }
    public async Task<string> UpdateVideo(VideoDto videoDto, CancellationToken cancellationToken)
    {
        if (_comSettings.VideoUrl == null)
        { throw new InvalidOperationException("VideoUrl is not configured."); }
        var url = $"{_comSettings.VideoUrl.TrimEnd('/')}/videos/{videoDto.Id}";
        using var response = await _httpClient.PutAsJsonAsync(url, videoDto, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return content;
    }
    public async Task<string> AddVideo(VideoDto videoDto, CancellationToken cancellationToken)
    {
        if (_comSettings.VideoUrl == null)
            throw new InvalidOperationException("VideoUrl is not configured.");

        var url = $"{_comSettings.VideoUrl.TrimEnd('/')}/Videos";

        using var response = await _httpClient.PostAsJsonAsync(
            url,
            videoDto,
            cancellationToken
        );



        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        response.EnsureSuccessStatusCode();
        return content;
    }

    public async Task<FileStream> GetThumbNail(string id)
    {
        var locationPrefix = _conf.GetValue<string>("ThumbnailLocation");

        const string query = "SELECT * FROM vid.Videos WHERE Id = @Id";

        using var connection = _context.CreateConnection();

        var selectedImage = await connection.QuerySingleOrDefaultAsync<Video>(query, new { Id = id });

        if (selectedImage == null)
        {
            throw new FileNotFoundException($"No video found with Id '{id}'.");
        }

        if (string.IsNullOrWhiteSpace(selectedImage.ThumbnailPath))
        {
            throw new FileNotFoundException($"Thumbnail path is missing for video '{id}'.");
        }

        var fullPath = Path.Combine(locationPrefix ?? string.Empty, selectedImage.ThumbnailPath);

        if (!System.IO.File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Thumbnail file not found: {fullPath}");
        }

        return System.IO.File.OpenRead(fullPath);
    }

    public async Task<string> getVideosForCurrentUser(
       VideoParams videoParams,
       CancellationToken cancellationToken)
    {
        var query = QueryString.Create(
           new Dictionary<string, string?>
           {
               ["pageNumber"] = videoParams.PageNumber.ToString(),
               ["pageSize"] = videoParams.PageSize.ToString(),
               ["category"] = videoParams.Category.ToString()
           });

        var url = $"{_comSettings.VideoUrl.TrimEnd('/')}/users/{videoParams.Id}/videos";
        try
        {
            // if i want to make a pagedlist in the future
            //using var response = await _httpClient.PostAsJsonAsync(url, query, cancellationToken);

            _logger.LogInformation("Calling video service URL: {Url}", url);

            using var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();


            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogInformation(
                "Video service responded. StatusCode: {StatusCode}, Body: {Body}",
                (int)response.StatusCode,
                content);


            return content;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "HTTP error calling video service. StatusCode: {StatusCode}, Message: {Message}, Inner: {Inner}",
                ex.StatusCode,
                ex.Message,
                ex.InnerException?.Message);

            throw;
        }



    }

    public async Task<string> AddUserToUserVideos(int Id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_comSettings.VideoUrl))
            throw new InvalidOperationException("No videoUrl configured.");

        var url = $"{_comSettings.VideoUrl.TrimEnd('/')}/users";
        using var response = await _httpClient.PostAsJsonAsync(
                    url,
                    new { UserId = Id, VideoId = 1, UploadedAt = "2026-04-15T10:23:12Z" },
                    cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task<string> AddVideoToUser(int VideoId, int UserId, CancellationToken cancellationToken)
    {
        if (_comSettings.VideoUrl == null)
            throw new InvalidOperationException("VideoUrl is not configured.");

        var url = $"{_comSettings.VideoUrl.TrimEnd('/')}/users/{UserId}/videos/{VideoId}";

        using var response = await _httpClient.PostAsJsonAsync(
                   url,
                   cancellationToken
               );
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return content;
    }

    public async Task<string> UpdateVideoAllowance(
     int[] videoAllowedToSee,
     int userId,
     CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_comSettings.VideoUrl))
            throw new InvalidOperationException("No videoUrl configured.");

        var url = $"{_comSettings.VideoUrl.TrimEnd('/')}/users/updateVideoAllowance";

        var payload = new UpdateVideoAllowanceRequest
        {
            VideoAllowance = videoAllowedToSee,
            UserId = userId
        };

        using var response = await _httpClient.PostAsJsonAsync(
            url,
            payload,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task<string> GetVideoByCategory(int id, CancellationToken cancellationToken)
    {
        if (_comSettings.VideoUrl == null)
        { throw new InvalidOperationException("VideoUrl is not configured."); }
        var url = $"{_comSettings.VideoUrl.TrimEnd('/')}/getVideosByCategory/{id}";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return content;
    }

    public async Task<string> GetSpecificVideoDetailsAsync(int id, CancellationToken cancellationToken)
    {
        if (_comSettings.VideoUrl == null)
        { throw new InvalidOperationException("VideoUrl is not configured."); }
        var url = $"{_comSettings.VideoUrl.TrimEnd('/')}/getSpecificVideoDetails/{id}";
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return content;
    }

    public async Task<string> DeleteVideo(int id, CancellationToken cancellationToken)
    {
        if (_comSettings.VideoUrl == null)
        { throw new InvalidOperationException("VideoUrl is not configured."); }
        var url = $"{_comSettings.VideoUrl.TrimEnd('/')}/Videos/delete/{id}";
        using var response = await _httpClient.DeleteAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return content;
    }
}
