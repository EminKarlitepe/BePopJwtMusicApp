namespace Core.Entities
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<SongGenre> SongGenres { get; set; }
    }
}
