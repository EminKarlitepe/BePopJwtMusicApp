namespace BepopStreamProject.Models
{
    public class AlbumViewModel
    {
        public int AlbumId { get; set; }
        public string Title { get; set; } = null!;
        public string? CoverImageUrl { get; set; }
        public DateTime ReleaseDate { get; set; }

        public string ArtistName { get; set; }
    }
}
