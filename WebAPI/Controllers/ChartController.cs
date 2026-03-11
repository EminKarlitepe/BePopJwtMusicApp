using DataAccess.Context;
using BepopStreamProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BepopStreamProject.Controllers
{
    public class ChartController : Controller
    {
        private readonly BepopDbContext _context;

        public ChartController(BepopDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string country, string genre)
        {
            var allSongsQuery = _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Album)
                .Include(s => s.SongGenres).ThenInclude(sg => sg.Genre)
                .AsQueryable();

            if (!string.IsNullOrEmpty(country) && country != "All countries")
            {
                allSongsQuery = allSongsQuery.Where(s => s.Artist.Country == country);
            }

            if (!string.IsNullOrEmpty(genre) && genre != "All genres")
            {
                allSongsQuery = allSongsQuery
                    .Where(s => s.SongGenres.Any(sg => sg.Genre.Name.ToLower() == genre.ToLower()));
            }

            var allSongs = allSongsQuery
                .Select(s => new SongViewModel
                {
                    SongId = s.SongId,
                    Title = s.Title,
                    ArtistName = s.Artist.Name,
                    FileUrl = s.FileUrl,
                    ImageUrl = s.Album.CoverImageUrl,
                    Level = s.Level,
                    Genres = new List<string>()
                })
                .ToList();

            var topTracks = _context.PlayHistories
                .Include(p => p.Song).ThenInclude(s => s.Artist)
                .Include(p => p.Song).ThenInclude(s => s.Album)
                .Include(p => p.Song).ThenInclude(s => s.SongGenres).ThenInclude(sg => sg.Genre)
                .AsEnumerable()
                .GroupBy(p => p.Song)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => new SongViewModel
                {
                    SongId = g.Key.SongId,
                    Title = g.Key.Title,
                    ArtistName = g.Key.Artist.Name,
                    FileUrl = g.Key.FileUrl,
                    ImageUrl = g.Key.Album.CoverImageUrl,
                    Level = g.Key.Level,
                    Genres = g.Key.SongGenres.Select(sg => sg.Genre.Name).ToList()
                })
                .ToList();

            var genres = _context.Genres
                .Select(g => g.Name)
                .Distinct()
                .ToList();

            var countries = _context.Artists
                .Select(a => a.Country)
                .Distinct()
                .ToList();

            var vm = new ChartViewModel
            {
                TopTracks = topTracks,
                AllSongs = allSongs,
                Genres = genres,
                Countries = countries
            };
            ViewBag.SelectedGenre = string.IsNullOrEmpty(genre) ? "All genres" : genre;
            ViewBag.SelectedCountry = string.IsNullOrEmpty(country) ? "All countries" : country;
            return View(vm);
        }


    }
}
