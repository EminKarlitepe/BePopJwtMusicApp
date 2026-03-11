namespace BepopStreamProject.Models
{
    public class ArtistPageViewModel
    {
        public List<ArtistViewModel> Artists { get; set; }
        public List<string> Countries { get; set; }
        public List<SongViewModel> TopTracks { get; set; }
    }
}
