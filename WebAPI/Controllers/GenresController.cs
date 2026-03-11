using DataAccess.Context;
using BepopStreamProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BepopStreamProject.Controllers
{
    public class GenresController : Controller
    {
        private readonly BepopDbContext _context;

        public GenresController(BepopDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? country, string? genre, string? q)
        {
            var songsQuery = _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Album)
                .Include(s => s.SongGenres).ThenInclude(sg => sg.Genre)
                .AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                songsQuery = songsQuery.Where(s =>
                    s.Title.Contains(q) ||
                    s.Artist.Name.Contains(q));
                ViewBag.SearchQuery = q;
            }

            if (!string.IsNullOrEmpty(country) && country != "All countries")
            {
                songsQuery = songsQuery.Where(s => s.Artist.Country == country);
            }

            if (!string.IsNullOrEmpty(genre) && genre != "All genres")
            {
                songsQuery = songsQuery.Where(s => s.SongGenres.Any(sg => sg.Genre.Name == genre));
            }

            var songs = songsQuery.Select(s => new SongViewModel
            {
                SongId = s.SongId,
                Title = s.Title,
                ArtistName = s.Artist.Name,
                FileUrl = s.FileUrl,
                ImageUrl = s.Album.CoverImageUrl,
                Level = s.Level,
                Genres = s.SongGenres.Select(sg => sg.Genre.Name).ToList()
            }).ToList();

            var vm = new ChartViewModel
            {
                AllSongs = songs,
                Genres = _context.Genres.Select(g => g.Name).ToList(),
                Countries = _context.Artists.Select(a => a.Country).Distinct().ToList()
            };

            ViewBag.SelectedGenre = string.IsNullOrEmpty(genre) ? "All genres" : genre;
            ViewBag.SelectedCountry = string.IsNullOrEmpty(country) ? "All countries" : country;

            return View(vm);
        }
    }
}