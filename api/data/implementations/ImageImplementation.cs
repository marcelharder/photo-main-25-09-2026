
using fotoservice.api.data.interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace api.data.implementations
{
    public class ImageImplementation : IImage
    {
        private ApplicationDbContext _context;
        private UserManager<AppUser> _userManager;
        private IHttpContextAccessor _ht;
        private readonly IMapper _mapper;
        private readonly IUsers _user;

        private readonly IConfiguration _conf;
        private readonly IOptions<ComSettings> _comSettings;
        private readonly ILogger<ImageImplementation> _logger;
        private readonly HttpClient _httpClient;

        public ImageImplementation(
            IConfiguration conf,
            ApplicationDbContext context,
            IMapper mapper,
            UserManager<AppUser> userManager,
            IHttpContextAccessor ht,
            IUsers user,
            IOptions<ComSettings> comSettings,
            ILogger<ImageImplementation> logger,
            HttpClient httpClient
        )
        {
            _user = user;
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
            _ht = ht;
            _conf = conf;
            _comSettings = comSettings;
            _logger = logger;
            _httpClient = httpClient;
        }



        public async Task<ImageDto> findImage(int Id, CancellationToken cancellationToken)
        {

            var url = $"{_comSettings.Value.PhotoContainerUrl.TrimEnd('/')}/Images/findImage/{Id}";
            try
            {
                // if i want to make a pagedlist in the future
                //using var response = await _httpClient.PostAsJsonAsync(url, query, cancellationToken);

                _logger.LogInformation("Calling video service URL: {Url}", url);

                using var response = await _httpClient.GetAsync(url, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogInformation(
                    "Photo service responded. StatusCode: {StatusCode}, Body: {Body}",
                    (int)response.StatusCode,
                    content);

                response.EnsureSuccessStatusCode();
                var result = JsonSerializer.Deserialize<ImageDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex,
                    "HTTP error calling photo service. StatusCode: {StatusCode}, Message: {Message}, Inner: {Inner}",
                    ex.StatusCode,
                    ex.Message,
                    ex.InnerException?.Message);

                throw;
            }



        }

        public async Task<PagedList<ImageDto>?> findImagesByUser(ImageParams ip, CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(_ht.HttpContext!.User);
            if (user == null)
            {
                _logger.LogWarning("User not found in GetImagesByUser");
                return null;
            }

            var query = new Test
            {
                userId = user.Id,
                AllowedCategories = user.PhotoAllowedToSee,
                Category = 0
            };

            var url = $"{_comSettings.Value.PhotoContainerUrl.TrimEnd('/')}/Images/getFilesForThisUser";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(url, query, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                response.EnsureSuccessStatusCode();
                var result = JsonSerializer.Deserialize<List<ImageDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return PagedList<ImageDto>.CreateAsync(result.AsQueryable(), ip.PageNumber, ip.PageSize);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex,
                    "HTTP error calling photo service. StatusCode: {StatusCode}, Message: {Message}, Inner: {Inner}",
                    ex.StatusCode,
                    ex.Message,
                    ex.InnerException?.Message);

                throw;
            }

        }

        public async Task<int> deleteImage(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> updateImage(ImageDto imagedto, CancellationToken cancellationToken
        )
        {
            if (_comSettings.Value.PhotoContainerUrl == null)
                throw new InvalidOperationException("PhotoContainerUrl is not configured.");
            var url = $"{_comSettings.Value.PhotoContainerUrl.TrimEnd('/')}/Images/updateImage";
            using var response = await _httpClient.PutAsJsonAsync(
                url,
                imagedto,
                cancellationToken
            );
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return content;
        }

        public async Task<string> addImage(ImageDto imDto, CancellationToken cancellationToken)
        {
            if (_comSettings.Value.PhotoContainerUrl == null)
                throw new InvalidOperationException("PhotoContainerUrl is not configured.");

            var url = $"{_comSettings.Value.PhotoContainerUrl.TrimEnd('/')}/Images/addImage";
            using var response = await _httpClient.PostAsJsonAsync(
                url,
                imDto,
                cancellationToken
            );
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            response.EnsureSuccessStatusCode();
            return content;
        }

        public async Task<CarouselDto> getCarouselData(int Id, CancellationToken cancellationToken)
        {
            if (_comSettings.Value.PhotoContainerUrl == null)
            { throw new InvalidOperationException("PhotoContainerUrl is not configured."); }

            var url = $"{_comSettings.Value.PhotoContainerUrl.TrimEnd('/')}/Images/getCarousel/{Id}";
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            
            var result = JsonSerializer.Deserialize<CarouselDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result;

        }

        public async Task<PagedList<ImageDto>> GetPhotosBySpecificCategory(ImageParams ip, CancellationToken cancellationToken)
        {
            if (_comSettings.Value.PhotoContainerUrl == null)
            { throw new InvalidOperationException("PhotoContainerUrl is not configured."); }

            var url = $"{_comSettings.Value.PhotoContainerUrl.TrimEnd('/')}/Images/getImagesByCategory/{ip.Category}";

            using var response = await _httpClient.GetAsync(url, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<ImageDto[]>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return PagedList<ImageDto>.CreateAsync(result.AsQueryable(), ip.PageNumber, ip.PageSize);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> GetTotalItemsInCategory(int category, CancellationToken cancellationToken)
        {
            if (_comSettings.Value.PhotoContainerUrl == null)
            { throw new InvalidOperationException("PhotoContainerUrl is not configured."); }

            var url = $"{_comSettings.Value.PhotoContainerUrl.TrimEnd('/')}/Images/getTotalNumberOfItemsInCategory/{category}";

            using var response = await _httpClient.GetAsync(url, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<int>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return result;



        }

       
    }
    public class Test
    {
        public int[]? AllowedCategories { get; set; }
        public int userId { get; set; }
        public int Category { get; set; }
    }
}
