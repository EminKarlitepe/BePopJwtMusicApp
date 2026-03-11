using Core.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BepopStreamProject.Models
{
    public class UploadSongViewModel
    {
        [Required(ErrorMessage = "Şarkı adı zorunludur.")]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Sanatçı seçmelisiniz.")]
        public int ArtistId { get; set; }

        public int? AlbumId { get; set; }

        [Range(1, 4)]
        public int Level { get; set; } = 1;

        public IFormFile? Mp3File { get; set; }
        public string? ExternalUrl { get; set; }
        public IFormFile? CoverFile { get; set; }

        public List<Artist> Artists { get; set; } = new();
        public List<Album> Albums { get; set; } = new();
        public List<Genre> Genres { get; set; } = new();


        public List<int> GenreIds { get; set; } = new();
    }
}