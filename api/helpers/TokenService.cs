

namespace fotoservice.data.helpers;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<AppUser> _userManager;

    public TokenService(IConfiguration config, UserManager<AppUser> userManager)
    {

        string? tokenKey = config["TokenKey"];
        if (string.IsNullOrEmpty(tokenKey)){ throw new ArgumentException("TokenKey cannot be null or empty");}
        else { _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)); }

        _userManager = userManager;
    }

    public async Task<string> CreateToken(AppUser user)
    {
        string? name = user.UserName;
        if(name != null){
        var claims = new List<Claim>{
              new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
              new Claim(JwtRegisteredClaimNames.UniqueName, name),
            };
        
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = System.DateTime.Now.AddHours(1),
            SigningCredentials = creds
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
        }
        else
        {
            return "error";
        }

    }
}
