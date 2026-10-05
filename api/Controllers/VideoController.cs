namespace api.Controllers;

using api.data.interfaces;
using api.Helpers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class VideoController : ControllerBase
{
    private readonly IVideoService _videoService;

    private readonly UserManager<AppUser> _userManager;



    public VideoController(IVideoService videoService, UserManager<AppUser> userManager)
    {
        _videoService = videoService;
        _userManager = userManager;
    }

    [HttpPost("getVideosByCategory")]
    public async Task<IActionResult> GetAllVideos(
        [FromQuery] VideoParams vp,
        CancellationToken cancellationToken
    )
    {
    try
        {
            var content = await _videoService.GetAllVideosAsync(vp, cancellationToken);
            return Content(content, "application/json");
        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            return StatusCode((int)ex.StatusCode.Value, ex.Message);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, $"Video service unavailable: {ex.Message}");
        }
    }

    [HttpPost("getVideosForUser")]
    public async Task<IActionResult> GetVideosForUser(
        [FromQuery] VideoParams vp,
        CancellationToken cancellationToken
    )
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        try
        {
            vp.Id = user.Id;


            var content = await _videoService.getVideosForCurrentUser(vp, cancellationToken);
            return Content(content, "application/json");
        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            return StatusCode((int)ex.StatusCode.Value, ex.Message);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, $"Video service unavailable: {ex.Message}");
        }
    }




    [HttpGet("getSpecificVideo/{id:int}", Name = "GetSpecificVideo")]
    public async Task<IActionResult> GetSpecificVideo(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _videoService.GetVideoStreamAsync(id, cancellationToken);
            return File(result.Stream, result.ContentType);
        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            return StatusCode((int)ex.StatusCode.Value, ex.Message);
        }
    }

    [HttpPut("updateVideo")]
    public async Task<IActionResult> UpdateVideo(VideoDto videoDto, CancellationToken cancellationToken)
    {
        var result = await _videoService.UpdateVideo(videoDto, cancellationToken);
        return Ok(result);
    }

    [HttpPost("addVideo")]
    public async Task<IActionResult> AddVideo(VideoDto videoDto, CancellationToken cancellationToken)
    {
        var result = await _videoService.AddVideo(videoDto, cancellationToken);
        return Ok(result);

    }

    /*  [HttpGet("addUserToVideos")]
     public async Task<IActionResult> AddUserToVideo()
     {

         CancellationToken ct = new CancellationToken();
         var user = await _userManager.GetUserAsync(User);
          if (user == null)
         return Unauthorized();

         var result = await _videoService.AddUserToUserVideos(user.Id,ct);
         return Ok(result);
     } */

    [HttpGet("addVideoToUser/{VideoId}/{UserId}", Name = "AddVideoToUser")]
    public async Task<IActionResult> AddVideoToUser(int VideoId, int UserId)
    {
        CancellationToken ct = new CancellationToken();
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var result = await _videoService.AddVideoToUser(VideoId, UserId, ct);
        return Ok(result);
    }

    [HttpGet("ImageThumbnailFile/{id}")]
    public async Task<IActionResult> getFullImageFile(int id)
    {
        var thumb = await _videoService.GetThumbNail(id.ToString());
        return File(thumb, "image/jpg");
    }
   
    [HttpGet("VideosByCategory/{id}")]
    public async Task<IActionResult> VideoByCategory(int id, CancellationToken cancellationToken)
    {
        var content = await _videoService.GetVideoByCategory(id, cancellationToken);
        return Ok(content);
    }   
    [HttpGet("getSpecificVideoDetails/{id:int}")]
    public async Task<IActionResult> GetSpecificVideoDetails(int id, CancellationToken cancellationToken)
    {
        try
        {
            var content = await _videoService.GetSpecificVideoDetailsAsync(id, cancellationToken);
            return Content(content, "application/json");
        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            return StatusCode((int)ex.StatusCode.Value, ex.Message);
        }
    }

    [HttpDelete("deleteVideo/{id:int}")]
    public async Task<IActionResult> DeleteVideo(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _videoService.DeleteVideo(id, cancellationToken);
            return Ok(result);
        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            return StatusCode((int)ex.StatusCode.Value, ex.Message);
        }
    }
    


}
