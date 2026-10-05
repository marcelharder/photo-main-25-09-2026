using fotoservice.api.data.interfaces;

namespace fotoservice.data.implementations;

public class UserRepo : IUsers
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    private readonly ApplicationDbContext _context;

    public UserRepo(UserManager<AppUser> userManager, IMapper mapper, ApplicationDbContext context)
    {
        _userManager = userManager;
        _mapper = mapper;
        _context = context;

    }

    public async Task<int> Delete(int id)
    {


        var selectedUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (selectedUser != null)
        {
            _context.Users.Remove(selectedUser);
            return 1;
        }
        else
        {
            return 0;
        }



    }

    public Task<AppUser> GetChefsByHospital(int center_id)
    {
        throw new NotImplementedException();
    }

    public async Task<UserForReturnDto?> GetUser(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        //var dto = new UserForReturnDto();
        if (user != null)
        {
            var us = _mapper.Map<UserForReturnDto>(user);
            us.PhoneNumber = await _userManager.GetPhoneNumberAsync(user);
            return us;
        }
        else { return null; }

    }

    public async Task<AppUser> GetUserByMail(string email)
    {
        if (email != null)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null) { return user; }
        }

        return null;
    }

    public Task<bool> GetUserLtk(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<UserForReturnDto>> GetUsers()
    {
        var dtolist = new List<UserForReturnDto>();
        var fullUsers = await _userManager.Users.ToListAsync();

        foreach (AppUser u in fullUsers)
        {

            dtolist.Add(_mapper.Map<UserForReturnDto>(u));
        }




        return dtolist;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

public async Task<bool> Update(UserForUpdateDto p)
{
    var user = await _userManager.FindByIdAsync(p.Id.ToString());
    if (user == null) return false;

    _mapper.Map(p, user);

    var result = await _userManager.UpdateAsync(user);
    return result.Succeeded;
}

    public Task<bool> UpdatePayment(DateTime d, int id)
    {
        throw new NotImplementedException();
    }
}