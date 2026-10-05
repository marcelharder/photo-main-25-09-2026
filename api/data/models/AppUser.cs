namespace fotoservice.data.models;
public class AppUser: IdentityUser<int>

    {
        public byte[]? PasswordSalt { get; set; }
        public string? Gender { get; set; }
        public string? PhotoUrl { get; set; }
        public int[]? PhotoAllowedToSee { get; set; }
        public int[]? VideoAllowedToSee { get; set; }
        public string? KnownAS {get; set;}
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public DateTime PaidTill { get; set; }
        public ICollection<AppUserRole>? UserRoles { get; set; }

       
    }

