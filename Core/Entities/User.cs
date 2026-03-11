namespace Core.Entities
{
    public class User
    {
        public Guid Uid { get; set; } = Guid.NewGuid();
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public int MembershipLevel { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? DisplayName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Bio { get; set; }
        public string? Country { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public ICollection<PlayHistory> PlayHistories { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
        public ICollection<UserFollow> Followers { get; set; }
        public ICollection<UserFollow> Following { get; set; }
        public ICollection<ArtistFollow> FollowingArtists { get; set; }
    }
}