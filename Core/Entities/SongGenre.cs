namespace Core.Entities
{
    public class SongGenre
    {
        public int SongId { get; set; }
        public int GenreId { get; set; }
        public Song Song { get; set; }
        public Genre Genre { get; set; }
    }
}
