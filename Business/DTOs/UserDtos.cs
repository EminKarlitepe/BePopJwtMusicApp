namespace Business.DTOs
{
    public class UserRegisterDto
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int MembershipLevel { get; set; } = 1;
    }

    public class UserLoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }

    public class UserLoginResultDto
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public int MembershipLevel { get; set; }
        public List<string> Roles { get; set; } = new();
        public string? Token { get; set; }
    }

    public class UserProfileDto
    {
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public string? Country { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}