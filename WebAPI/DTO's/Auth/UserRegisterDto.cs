namespace BepopStreamProject.DTO_s.Auth
{
    public class UserRegisterDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int MembershipLevel { get; set; } // Bu alan mutlaka olmalı
    }
}
