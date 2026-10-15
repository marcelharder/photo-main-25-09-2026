using CloudinaryDotNet.Actions;
using IronSoftware.Drawing;
using System.Net.Mime;

namespace api.Controllers;

public class ImagesController : BaseApiController
{
    private static readonly HashSet<string> SupportedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".bmp",
        ".gif",
        ".jpeg",
        ".jpg",
        ".png",
        ".tif",
        ".tiff",
        ".webp"
    };

    private readonly IImage _image;

    private readonly IConfiguration _conf;

    public ImagesController(
        IImage image,
        IConfiguration conf
     )
    {
        _image = image;
        _conf = conf;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("browseNasImages")]
    public IActionResult BrowseNasImages([FromQuery] string? directory = null)
    {
        var root = GetNasBrowseRoot();
        if (!TryGetSafeNasPath(root, directory, out var directoryPath))
            return BadRequest("The requested path is outside the configured NAS image directory.");
        if (!Directory.Exists(directoryPath))
            return NotFound("NAS directory not found.");

        var directories = Directory.EnumerateDirectories(directoryPath)
            .Where(path => (System.IO.File.GetAttributes(path) & FileAttributes.ReparsePoint) == 0)
            .Select(path => new NasImageEntry(
                Path.GetFileName(path),
                GetRelativeNasPath(root, path),
                true))
            .OrderBy(entry => entry.name, StringComparer.OrdinalIgnoreCase);

        var files = Directory.EnumerateFiles(directoryPath)
            .Where(path =>
                SupportedImageExtensions.Contains(Path.GetExtension(path)) &&
                (System.IO.File.GetAttributes(path) & FileAttributes.ReparsePoint) == 0)
            .Select(path => new NasImageEntry(
                Path.GetFileName(path),
                GetRelativeNasPath(root, path),
                false))
            .OrderBy(entry => entry.name, StringComparer.OrdinalIgnoreCase);

        return Ok(directories.Concat(files));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("nasImagePreview")]
    public IActionResult GetNasImagePreview([FromQuery] string? path)
    {
        var root = GetNasBrowseRoot();
        if (string.IsNullOrWhiteSpace(path))
            return BadRequest("An image path is required.");
        if (!TryGetSafeNasPath(root, path, out var filePath))
            return BadRequest("The requested path is outside the configured NAS image directory.");
        if (!SupportedImageExtensions.Contains(Path.GetExtension(filePath)))
            return BadRequest("Unsupported image type.");
        if (!System.IO.File.Exists(filePath))
            return NotFound("NAS image not found.");
        if ((System.IO.File.GetAttributes(filePath) & FileAttributes.ReparsePoint) != 0)
            return BadRequest("NAS links cannot be previewed.");

        return PhysicalFile(filePath, GetImageContentType(filePath));
    }

    private string GetNasBrowseRoot()
    {
        var configuredRoot = _conf.GetValue<string>("NfsLocation");
        if (string.IsNullOrWhiteSpace(configuredRoot))
            throw new InvalidOperationException("NfsLocation is not configured.");

        var imageRoot = Path.GetFullPath(configuredRoot);
        var root = Directory.GetParent(Path.TrimEndingDirectorySeparator(imageRoot))?.FullName
            ?? throw new InvalidOperationException("NfsLocation must be a subdirectory of the NAS browse directory.");
        if (!Directory.Exists(root))
            throw new DirectoryNotFoundException($"The NAS browse directory does not exist: {root}");

        return root;
    }

    private static bool TryGetSafeNasPath(string root, string? relativePath, out string candidate)
    {
        if (relativePath?.Contains('\0') == true)
        {
            candidate = string.Empty;
            return false;
        }

        candidate = Path.GetFullPath(Path.Combine(root, relativePath ?? string.Empty));
        var rootWithSeparator = Path.EndsInDirectorySeparator(root) ? root : root + Path.DirectorySeparatorChar;
        return candidate.Equals(root, StringComparison.Ordinal) ||
            candidate.StartsWith(rootWithSeparator, StringComparison.Ordinal);
    }

    private static string GetRelativeNasPath(string root, string fullPath)
    {
        return Path.GetRelativePath(root, fullPath).Replace(Path.DirectorySeparatorChar, '/');
    }

    private static string GetImageContentType(string path)
    {
        return Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".bmp" => "image/bmp",
            ".gif" => "image/gif",
            ".jpeg" or ".jpg" => "image/jpeg",
            ".png" => "image/png",
            ".tif" or ".tiff" => "image/tiff",
            ".webp" => "image/webp",
            _ => MediaTypeNames.Application.Octet
        };
    }

    [HttpGet("getImageFile/{id}")]
    public async Task<IActionResult> getImageFile(int id)
    {
        const int maxWidth = 400;
        const int maxHeight = 400;
        var locationPrefix = _conf.GetValue<string>("NfsLocation");

        var selectedImage = await _image.findImage(id, new CancellationToken());
        var img = System.IO.File.OpenRead(locationPrefix + selectedImage.ImageUrl);

        using (var image = SixLabors.ImageSharp.Image.Load(img))
        {
            var resizeOptions = new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new IronSoftware.Drawing.Size (maxWidth, maxHeight)
            };
            image.Mutate(x => x.Resize(resizeOptions));
            using var ms = new MemoryStream();
            image.SaveAsJpeg(ms, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
            {
                Quality = 85
            });

            return File(ms.ToArray(), "image/jpeg");
        }

    }

    [HttpGet("getFullImageFile/{id}")]
    public async Task<IActionResult> getFullImageFile(int id)
    {
        var locationPrefix = _conf.GetValue<string>("NfsLocation");
        var selectedImage = await _image.findImage(id, new CancellationToken());
        var img = System.IO.File.OpenRead(locationPrefix + selectedImage.ImageUrl);
        return File(img, "image/jpg");
    }

    [HttpGet("getImagesByCategory")]
    public async Task<ActionResult<PagedList<ImageDto>>> GetImagesByCat([FromQuery] ImageParams imgP, CancellationToken cancellationToken)
    {
        var plImages = await _image.GetPhotosBySpecificCategory(imgP, cancellationToken);
        if (plImages == null)
            return BadRequest("No images found");

        var totalItems = plImages.TotalCount;
        var totalPages = (int)Math.Ceiling(totalItems / (double)imgP.PageSize);

        Response.AddPaginationHeader(
            new PaginationHeader(
                imgP.PageNumber,
                imgP.PageSize,
                totalItems,
                totalPages
            )
        );
        return Ok(plImages);
    }

    [Authorize]
    [HttpGet("getFilesForThisUser")]
    public async Task<ActionResult<PagedList<ImageDto>>> GetImagesByUser([FromQuery] ImageParams ip)
    {
        var plImages = await _image.findImagesByUser(ip, new CancellationToken());

        if (plImages == null)
            return BadRequest("No images found");

        var totalItems = plImages.TotalCount;
        var totalPages = (int)Math.Ceiling(totalItems / (double)ip.PageSize);

        Response.AddPaginationHeader(
            new PaginationHeader(
                ip.PageNumber,
                ip.PageSize,
                totalItems,
                totalPages
            )
        );
        return Ok(plImages);
    }

    [HttpPost("addImage")]
    public async Task<ActionResult<string>> AddImage([FromBody] fotoservice.data.models.Image im)
    {
        var imgDto = new ImageDto
        {
            ImageUrl = im.ImageUrl,
            YearTaken = im.YearTaken,
            Location = im.Location,
            Familie = im.Familie,
            Category = im.Category,
            Quality = im.Quality,
            Series = im.Series,
            Spare1 = im.Spare1,
            Spare2 = im.Spare2,
            Spare3 = im.Spare3,
        };
        var result = await _image.addImage(imgDto, new CancellationToken());
        return Ok(result);
    }

    [HttpDelete("deleteImage/{id}")]
    public async Task<ActionResult<string>> DeleteImage(int id)
    {
        var result = await _image.deleteImage(id, new CancellationToken());
        return Ok(result);
    }

    [HttpPut("updateImage")]
    public async Task<ActionResult<string>> UpdateImage(ImageDto imagedto,CancellationToken cancellationToken)
    {
       var result = await _image.updateImage(imagedto,cancellationToken);
       return Ok(result);
    }

    [HttpGet("findImage/{Id}", Name = "GetImage")]
    public async Task<ActionResult<ImageDto>> FindImage(int Id)
    {
        return await _image.findImage(Id, new CancellationToken());
    }

    [HttpGet("getCarousel/{Id}")]
    public async Task<ActionResult<CarouselDto>> getCarousel(int Id, CancellationToken cancellationToken)
    {
        return await _image.getCarouselData(Id, cancellationToken);
    }
}

public sealed record NasImageEntry(string name, string path, bool isDirectory);
