using CloudinaryDotNet.Actions;
using IronSoftware.Drawing;

namespace api.Controllers;

public class ImagesController : BaseApiController
{
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


