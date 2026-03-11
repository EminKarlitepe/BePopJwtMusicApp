namespace BepopStreamProject.Models
{
    public class ArtistDetailViewModel
    {
        public int ArtistId { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string ImageUrl { get; set; }

        public List<SongViewModel> Songs { get; set; }

        public List<SongViewModel> TopTracks { get; set; }

        public List<AlbumViewModel> Albums { get; set; }

        public int FollowersCount { get; set; }
        public bool IsFollowing { get; set; }
    }
}
