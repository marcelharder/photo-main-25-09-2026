namespace fotoservice.data.dtos;

public class UserForRegisterDto
    {
        [Required]
        public required string UserName { get; set; }
        [Required]
        [StringLength(80, MinimumLength = 6, ErrorMessage="Password should be minimum 6 and max 80 char")]
        public required string Password { get; set; }
        public string? Country {get; set;}
        public string? City {get; set;}
        public required string Email {get; set;}
        public required string gender {get; set;}
        public string? KnownAs {get; set;}
        public required string PhoneNumber {get; set;}
        public bool Active {get; set;}
       
       

    }