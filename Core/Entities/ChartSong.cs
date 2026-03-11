namespace Core.Entities
{
    public class ChartSong
    {
        public int ChartSongId { get; set; }
        public int ChartId { get; set; }
        public int SongId { get; set; }
        public int Position { get; set; }
        public int Score { get; set; }
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public Chart Chart { get; set; }
        public Song Song { get; set; }
    }
}
