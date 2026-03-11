namespace BepopStreamProject.Models
{
    public class ChartViewModel
    {
        public List<SongViewModel> TopTracks { get; set; }
        public List<SongViewModel> AllSongs { get; set; }

        public List<string> Genres { get; set; }
        public List<string> Countries { get; set; }
    }
}
