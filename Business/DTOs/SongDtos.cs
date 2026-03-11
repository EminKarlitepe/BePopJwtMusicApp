namespace Business.DTOs
{
    public class SongPlayResultDto
    {
        public bool CanPlay { get; set; }
        public string? FileUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }
}