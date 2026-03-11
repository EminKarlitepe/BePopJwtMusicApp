using DataAccess.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Core.Entities;
using BepopStreamProject.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BepopStreamProject.Controllers
{
    [Authorize]
    public class PlaylistsController : Controller
    {
        private readonly BepopDbContext _context;

        public PlaylistsController(BepopDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = User.GetUserId();
            if (userId <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var playlists = _context.Playlists
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                .ToList();

            return View(playlists);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string Name)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ModelState.AddModelError("Name", "Liste adı boş olamaz.");
                return View();
            }

            var userId = User.GetUserId();
            if (userId <= 0)
            {
                return Unauthorized();
            }

            var playlist = new Playlist
            {
                Name = Name.Trim(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Playlists.Add(playlist);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var userId = User.GetUserId();
            if (userId <= 0)
            {
                return RedirectToAction("Login", "Auth");
            }

            var playlist = _context.Playlists
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                        .ThenInclude(s => s.Artist)
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                        .ThenInclude(s => s.Album)
                .FirstOrDefault(p => p.PlaylistId == id && p.UserId == userId);

            if (playlist == null) return NotFound();

            return View(playlist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var userId = User.GetUserId();
            if (userId <= 0)
            {
                return Unauthorized();
            }

            var playlist = _context.Playlists
                .Include(p => p.PlaylistSongs)
                .FirstOrDefault(p => p.PlaylistId == id && p.UserId == userId);

            if (playlist != null)
            {
                _context.PlaylistSongs.RemoveRange(playlist.PlaylistSongs);
                _context.Playlists.Remove(playlist);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSong(int playlistId, int songId)
        {
            var userId = User.GetUserId();
            if (userId <= 0)
            {
                return Unauthorized();
            }

            var playlist = _context.Playlists.FirstOrDefault(p => p.PlaylistId == playlistId && p.UserId == userId);
            if (playlist == null)
            {
                return NotFound();
            }

            var exists = _context.PlaylistSongs.Any(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
            if (!exists)
            {
                _context.PlaylistSongs.Add(new PlaylistSong
                {
                    PlaylistId = playlistId,
                    SongId = songId
                });
                _context.SaveChanges();
            }

            return RedirectToAction("Details", new { id = playlistId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveSong(int playlistId, int songId)
        {
            var userId = User.GetUserId();
            if (userId <= 0)
            {
                return Unauthorized();
            }

            var playlist = _context.Playlists.FirstOrDefault(p => p.PlaylistId == playlistId && p.UserId == userId);
            if (playlist == null)
            {
                return NotFound();
            }

            var link = _context.PlaylistSongs.FirstOrDefault(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
            if (link != null)
            {
                _context.PlaylistSongs.Remove(link);
                _context.SaveChanges();
            }

            return RedirectToAction("Details", new { id = playlistId });
        }

        [HttpGet]
        public IActionResult GetUserPlaylists()
        {
            var userId = User.GetUserId();
            if (userId <= 0) return Unauthorized();

            var playlists = _context.Playlists
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    p.PlaylistId,
                    p.Name,
                    SongCount = p.PlaylistSongs.Count()
                })
                .ToList();

            return Json(playlists);
        }

        [HttpPost]
        public IActionResult AddFromAjax(int playlistId, int songId)
        {
            var userId = User.GetUserId();
            if (userId <= 0)
            {
                return Unauthorized();
            }

            var playlist = _context.Playlists.FirstOrDefault(p => p.PlaylistId == playlistId && p.UserId == userId);
            if (playlist == null)
            {
                return NotFound();
            }

            var exists = _context.PlaylistSongs.Any(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
            if (!exists)
            {
                _context.PlaylistSongs.Add(new PlaylistSong
                {
                    PlaylistId = playlistId,
                    SongId = songId
                });
                _context.SaveChanges();
            }

            return Json(new { success = true });
        }
    }
}