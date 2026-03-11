namespace Core.Entities
{
    public class Playlist
    {
        public int PlaylistId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public ICollection<PlaylistSong> PlaylistSongs { get; set; }
    }
}
