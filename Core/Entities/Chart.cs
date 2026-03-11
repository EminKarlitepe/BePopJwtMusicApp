namespace Core.Entities
{
    public class Chart
    {
        public int ChartId { get; set; }
        public string Name { get; set; } = null!;
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; }
        public string? Genre { get; set; }
        public ICollection<ChartSong> ChartSongs { get; set; }
    }
}
