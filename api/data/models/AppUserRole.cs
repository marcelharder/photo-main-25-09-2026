namespace fotoservice.data.models;

    public class AppUserRole: IdentityUserRole<int>
    {
        public AppUser? User { get; set; }
        public AppRole? Role { get; set; }
    }
