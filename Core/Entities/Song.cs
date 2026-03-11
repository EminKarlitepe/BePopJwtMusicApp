namespace Core.Entities
{
    public class Song
    {
        public int SongId { get; set; }
        public string Title { get; set; } = null!;
        public int ArtistId { get; set; }
        public int? AlbumId { get; set; }
        public string? AlbumCoverUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FileUrl { get; set; } = null!;
        public int Level { get; set; }

        public Artist Artist { get; set; }
        public Album? Album { get; set; }

        public ICollection<PlayHistory> PlayHistories { get; set; }
        public ICollection<SongGenre> SongGenres { get; set; }
        public ICollection<PlaylistSong> PlaylistSongs { get; set; }
        public ICollection<ChartSong> ChartSongs { get; set; }
    }
}
