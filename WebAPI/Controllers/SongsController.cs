using DataAccess.Context;
using Core.Entities;
using BepopStreamProject.Helpers;
using BepopStreamProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BepopStreamProject.Controllers
{
    public class SongsController : Controller
    {
        private readonly BepopDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SongsController(BepopDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Play(int songId)
        {
            var userId = User.GetUserId();

            // Level'ı DB'den oku — cookie'ye güvenme
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            var userLevel = user?.MembershipLevel ?? 0;

            var song = _context.Songs.FirstOrDefault(s => s.SongId == songId);
            if (song == null) return NotFound();

            if (userLevel >= song.Level)
            {
                if (userId > 0)
                {
                    _context.PlayHistories.Add(new PlayHistory
                    {
                        UserId = userId,
                        SongId = songId,
                        PlayedAt = DateTime.Now
                    });
                    _context.SaveChanges();
                }

                return Json(new { url = song.FileUrl });
            }
            else
            {
                var isAjax = string.Equals(
                    Request.Headers["X-Requested-With"],
                    "XMLHttpRequest",
                    StringComparison.OrdinalIgnoreCase);

                if (isAjax)
                {
                    Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Json(new { error = "Bu şarkıyı dinlemek için paket yükseltmeniz gerekiyor." });
                }

                TempData["Error"] = "Bu şarkıyı dinlemek için paket yükseltmeniz gerekiyor.";
                return RedirectToAction("Upgrade", "Membership");
            }
        }

        public IActionResult Details(int id)
        {
            var userId = User.GetUserId();
            if (userId == 0)
                return RedirectToAction("Login", "Auth");

            var song = _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Album)
                    .ThenInclude(a => a!.Songs)
                .Include(s => s.SongGenres)
                    .ThenInclude(sg => sg.Genre)
                .FirstOrDefault(s => s.SongId == id);

            if (song == null) return NotFound();

            var artistName = song.Artist?.Name ?? "Bilinmeyen Sanatçı";

            var moreBySameArtist = _context.Songs
                .Include(s => s.Album)
                .Where(s => s.ArtistId == song.ArtistId && s.SongId != id)
                .OrderByDescending(s => s.CreatedAt)
                .Take(6)
                .Select(s => new SongViewModel
                {
                    SongId = s.SongId,
                    Title = s.Title,
                    ArtistName = artistName,
                    FileUrl = s.FileUrl,
                    Level = s.Level,
                    ImageUrl = s.Album != null ? s.Album.CoverImageUrl : null
                })
                .ToList();

            var playCount = _context.PlayHistories.Count(p => p.SongId == id);

            // UserLevel'ı da DB'den oku
            var dbUser = _context.Users.FirstOrDefault(u => u.UserId == userId);
            var userLevel = dbUser?.MembershipLevel ?? 0;

            var model = new SongDetailViewModel
            {
                SongId = song.SongId,
                Title = song.Title,
                ArtistName = artistName,
                ArtistId = song.ArtistId,
                AlbumTitle = song.Album?.Title,
                AlbumId = song.AlbumId,
                CoverImageUrl = song.Album?.CoverImageUrl ?? "/Bebop/assets/img/default-cover.jpg",
                FileUrl = song.FileUrl,
                Level = song.Level,
                CreatedAt = song.CreatedAt,
                Genres = song.SongGenres.Select(sg => sg.Genre?.Name).ToList(),
                MoreByArtist = moreBySameArtist,
                PlayCount = playCount,
                UserLevel = userLevel
            };

            return View(model);
        }
    }
}