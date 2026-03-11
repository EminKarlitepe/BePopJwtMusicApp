namespace Core.Entities
{
    public class ArtistFollow
    {
        public int UserId { get; set; }
        public int ArtistId { get; set; }

        public User User { get; set; }
        public Artist Artist { get; set; }
    }
}

