namespace fotoservice.data.interfaces;

    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
