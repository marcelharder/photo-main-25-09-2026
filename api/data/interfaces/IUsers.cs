namespace fotoservice.api.data.interfaces;

public interface IUsers
{

      Task<List<UserForReturnDto>> GetUsers();
      Task<AppUser> GetChefsByHospital(int center_id);
      Task<UserForReturnDto?> GetUser(int id);
      Task<AppUser> GetUserByMail(string email);
      Task<bool> GetUserLtk(int id);
      Task<bool> UpdatePayment(DateTime d, int id);
      Task<bool> SaveChangesAsync();
      Task<bool> Update(UserForUpdateDto p);
      Task<int> Delete(int id);
}