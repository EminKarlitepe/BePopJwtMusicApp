using Core.Entities;

namespace BepopStreamProject.Models
{
    public class DiscoverViewModel
    {
        public List<SongViewModel> TopTracks { get; set; }
        public List<AlbumViewModel> FeaturedAlbums { get; set; }
        public List<SongViewModel> RecentlyAdded { get; set; }
        public List<SongViewModel> RecommendedSongs { get; set; }

    }
}
