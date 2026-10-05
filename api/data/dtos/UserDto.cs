namespace fotoservice.data.dtos;
    public class UserDto
    {
        public required string UserName { get; set; }
        public required string Token{get; set;}
        public int Id { get; set; }
        public DateTime paidTill { get; set; } 
        public bool IsAdmin { get; set; }
    }
