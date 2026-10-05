namespace fotoservice.data.dtos;

public class UserForReturnDto
    {
       
        public required int Id {get; set;}
        public  string? UserName { get; set; }
        public int[]? PhotoAllowedToSee {get; set;}
        public int[]? VideoAllowedToSee {get; set;}
        public  string? Email {get; set;}
        public DateTime Created {get; set;}
        public  string? gender {get; set;}
        public  string? PhoneNumber {get; set;}
        public DateTime paidTill { get; set; } 
        
    }  
       