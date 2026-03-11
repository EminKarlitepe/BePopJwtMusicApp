namespace Core.Entities
{
    public class Artist
    {
        public int ArtistId { get; set; }
        public string Name { get; set; } = null!;
        public string? Bio { get; set; }
        public string? Country { get; set; }
        public string? ProfileImageUrl { get; set; }
        public ICollection<Album> Albums { get; set; }
        public ICollection<Song> Songs { get; set; }
        public ICollection<ArtistFollow> Followers { get; set; }
    }
}
