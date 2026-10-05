namespace api.Controllers;

public class AccountController : BaseApiController
{

    private readonly ITokenService _ts;
    private static readonly HttpClient client = new HttpClient();
    private readonly UserManager<AppUser> _manager;
    private readonly SignInManager<AppUser> _signIn;
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _hostEnvironment;

    public AccountController(
        ITokenService ts,
        IConfiguration config,
        UserManager<AppUser> manager,
        IWebHostEnvironment hostEnvironment,
        SignInManager<AppUser> signIn)
    {
        _config = config;
        _manager = manager;
        _signIn = signIn;
        _ts = ts;
        _hostEnvironment = hostEnvironment;



    }

    [HttpGet("checkIfUserExists/{email}")]
    public async Task<int> Userexists(string email)
    {
        var result = 0;
        var user = await _manager.Users.FirstOrDefaultAsync(x => x.Email == email);
        if (user != null) { result = 1; }
        return result;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(UserForRegisterDto registerDto)
    {
        var user = await _manager.Users.SingleOrDefaultAsync(x => x.UserName == registerDto.UserName.ToLower());
        if (user != null) { return BadRequest("User already exists ..."); }

        user = new AppUser
        {
            UserName = registerDto.UserName.ToLower(),
            PhoneNumber = registerDto.PhoneNumber,
            Created = DateTime.Now,
            LastActive = DateTime.Now,
            PaidTill = DateTime.Now.AddDays(90),
            Email = registerDto.Email.ToLower(),
            Gender = "Male",
        };

        var result = await _manager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded) { return BadRequest(result.Errors); }

        var roleResult = await _manager.AddToRoleAsync(user, "Surgery");
        if (!roleResult.Succeeded) { return BadRequest(roleResult.Errors); }

        return new UserDto
        {
            UserName = user.UserName,
            Token = await _ts.CreateToken(user),
            Id = user.Id,
            IsAdmin = await _manager.IsInRoleAsync(user, "Admin"),
        };
    }



    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(UserForLoginDto ufl)
    {
        var user = await _manager.Users.SingleOrDefaultAsync(x => x.UserName == ufl.UserName.ToLower());
        if (user == null) return Unauthorized();

        var result = await _signIn.CheckPasswordSignInAsync(user, ufl.password, false);
        if (!result.Succeeded) return Unauthorized();


        return new UserDto
        {
            UserName = user.UserName!,
            Token = await _ts.CreateToken(user),
            Id = user.Id,
            IsAdmin = await _manager.IsInRoleAsync(user, "Admin"),
        };
    }

    [HttpPut("changePassword")]
    public async Task<IActionResult> ChangePwd([FromBody] ChangePasswordDto ufl)
    {
        var user = await _manager.FindByEmailAsync(ufl.Email);
        if (user == null)
            return BadRequest("Invalid Request");

        var resultp = await _signIn.CheckPasswordSignInAsync(user, ufl.CurrentPassword, false);
        if (!resultp.Succeeded) return BadRequest("Pls use correct password ...");

        var result = await _manager.ChangePasswordAsync(user, ufl.CurrentPassword, ufl.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => new
            {
                e.Code,
                e.Description
            }));
        }

        return Ok("Password changed");
    }






}
