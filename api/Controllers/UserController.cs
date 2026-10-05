using api.data.interfaces;
using fotoservice.api.data.interfaces;

namespace api.Controllers;

public class UserController : BaseApiController
{
    private readonly IUsers _users;
    private readonly IVideoService _videoService;

    public UserController(IUsers users, IVideoService videoService)
    {
        _users = users;
        _videoService = videoService;
    }

    [HttpGet("getAllUsers")]
    public async Task<IActionResult> allUsers()
    {
        return Ok(await _users.GetUsers());
    }

    [HttpDelete("RemoveUser/{id}")]
    public async Task<IActionResult> deleteUser(int id)
    {
        var result = await _users.Delete(id);
        if (await _users.SaveChangesAsync())
        {
            return Ok("User removed");
        }
        return BadRequest("User was not deleted");
    }

    /*  [HttpPut("UpdateUser")]
     public async Task<IActionResult> updateUser([FromBody]UserForUpdateDto us, CancellationToken cancellationToken)
     {
         // parse  the array of int to json
         string VideoAllowance = us.VideoAllowedToSee

         await _videoService.UpdateVideoAllowance(us.VideoAllowedToSee,UserId,cancellationToken);



         if(await _users.Update(us)) return Ok("User updated");
         return BadRequest("User was not updated");

     } */

    [HttpPut("UpdateUser")]
    public async Task<IActionResult> UpdateUser(
    [FromBody] UserForUpdateDto us,
    CancellationToken cancellationToken)
    {
        if (us == null)
            return BadRequest("Request body is required.");

        // If you need JSON text from the int array/list
        // string videoAllowanceJson = System.Text.Json.JsonSerializer.Serialize(us.VideoAllowedToSee);

        await _videoService.UpdateVideoAllowance(us.VideoAllowedToSee, us.Id, cancellationToken);

        var updated = await _users.Update(us);

        if (updated)
            return Ok("User updated");

        return BadRequest("User was not updated");
    }

    [HttpGet("SpecificUser/{id}")]
    public async Task<IActionResult> getUser(int id)
    {
        return Ok(await _users.GetUser(id));
    }
}
