namespace BepopStreamProject.Models
{
    public class SongViewModel
    {
        public int SongId { get; set; }
        public string Title { get; set; } = null!;
        public string? ArtistName { get; set; }
        public string? FileUrl { get; set; }
        public int Level { get; set; }
        public string? ImageUrl { get; set; }
        public List<string?> Genres { get; set; } = new();
    }
}