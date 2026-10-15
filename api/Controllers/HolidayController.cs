using System.Net;
using api.data.implementations;

namespace api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class HolidayController : ControllerBase
{
    private readonly HolidayService _holidayService;

    public HolidayController(HolidayService holidayService)
    {
        _holidayService = holidayService;
    }

    [HttpGet("getAllHolidays")]
    public async Task<IActionResult> GetAllHolidays(
        [FromQuery] HolidaysParams cp,
        CancellationToken cancellationToken)
    {


        try
        {
            var content = await _holidayService.GetAllHolidays(cp, cancellationToken);

            return Ok(content);
        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            return StatusCode((int)ex.StatusCode.Value, ex.Message);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, $"Holiday service unavailable: {ex.Message}");
        }
    }

    [HttpGet("getImage/{id}")]
    public async Task<IActionResult> GetImage(
    int id,
    CancellationToken cancellationToken)
    {
        try
        {
            var image = await _holidayService.GetImage(id, cancellationToken);

            return image;
        }
        catch (HttpRequestException ex) when (
            ex.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            return StatusCode((int)ex.StatusCode.Value, ex.Message);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                $"Holiday service unavailable: {ex.Message}");
        }
    }

    [HttpPost("addHoliday")]
    public async Task<IActionResult> AddHoliday(
        [FromBody] Holiday holiday,
        CancellationToken cancellationToken)
    {
        if (holiday is null)
        {
            return BadRequest("Request body is required.");
        }

        try
        {
            var result = await _holidayService.CreateHoliday(holiday, cancellationToken);
            return Ok(result);
        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            return StatusCode((int)ex.StatusCode.Value, ex.Message);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                $"Holiday service unavailable: {ex.Message}");
        }
    }

    [HttpDelete("deleteHoliday/{id}")]
    public async Task<IActionResult> DeleteHoliday(
    int id,
    CancellationToken cancellationToken)
    {
       try
        {
            var result = await _holidayService.DeleteHoliday(id, cancellationToken);

            return NoContent();
        }
        catch (HttpRequestException ex) when (
            ex.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            return StatusCode((int)ex.StatusCode.Value, ex.Message);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                $"Holiday service unavailable: {ex.Message}");
        } 
    }






}