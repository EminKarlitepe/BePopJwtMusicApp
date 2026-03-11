namespace BepopStreamProject.Models
{
    public class SongDetailViewModel
    {
        public int SongId { get; set; }
        public string Title { get; set; } = null!;
        public string ArtistName { get; set; } = null!;
        public int ArtistId { get; set; }
        public string? AlbumTitle { get; set; }
        public int? AlbumId { get; set; }
        public string CoverImageUrl { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public int Level { get; set; }
        public int UserLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PlayCount { get; set; }
        public List<string?> Genres { get; set; } = new();
        public List<SongViewModel> MoreByArtist { get; set; } = new();
    }
}