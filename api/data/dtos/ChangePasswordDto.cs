namespace fotoservice.data.dtos;
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }

        public required string Email { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
        public required string CurrentPassword { get; set; }
    }
