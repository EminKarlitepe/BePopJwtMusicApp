using Business.Abstract;
using Core.Entities;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace Business.Concrete
{
    public class PlaylistManager : IPlaylistService
    {
        private readonly BepopDbContext _context;

        public PlaylistManager(BepopDbContext context)
        {
            _context = context;
        }

        public List<Playlist> GetByUser(int userId)
        {
            return _context.Playlists
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Include(p => p.PlaylistSongs).ThenInclude(ps => ps.Song)
                .ToList();
        }

        public Playlist? GetById(int playlistId, int userId)
        {
            return _context.Playlists
                .Include(p => p.PlaylistSongs).ThenInclude(ps => ps.Song).ThenInclude(s => s.Artist)
                .Include(p => p.PlaylistSongs).ThenInclude(ps => ps.Song).ThenInclude(s => s.Album)
                .FirstOrDefault(p => p.PlaylistId == playlistId && p.UserId == userId);
        }

        public Playlist Create(int userId, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Liste adı boş olamaz.");

            var playlist = new Playlist
            {
                Name = name.Trim(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Playlists.Add(playlist);
            _context.SaveChanges();
            return playlist;
        }

        public void Delete(int playlistId, int userId)
        {
            var playlist = _context.Playlists
                .Include(p => p.PlaylistSongs)
                .FirstOrDefault(p => p.PlaylistId == playlistId && p.UserId == userId);

            if (playlist == null) return;

            _context.PlaylistSongs.RemoveRange(playlist.PlaylistSongs);
            _context.Playlists.Remove(playlist);
            _context.SaveChanges();
        }

        public bool AddSong(int playlistId, int songId, int userId)
        {
            var playlist = _context.Playlists.FirstOrDefault(p => p.PlaylistId == playlistId && p.UserId == userId);
            if (playlist == null) return false;

            var exists = _context.PlaylistSongs.Any(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
            if (exists) return false;

            _context.PlaylistSongs.Add(new PlaylistSong { PlaylistId = playlistId, SongId = songId });
            _context.SaveChanges();
            return true;
        }

        public bool RemoveSong(int playlistId, int songId, int userId)
        {
            var playlist = _context.Playlists.FirstOrDefault(p => p.PlaylistId == playlistId && p.UserId == userId);
            if (playlist == null) return false;

            var link = _context.PlaylistSongs.FirstOrDefault(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
            if (link == null) return false;

            _context.PlaylistSongs.Remove(link);
            _context.SaveChanges();
            return true;
        }

        public List<object> GetUserPlaylistsSummary(int userId)
        {
            return _context.Playlists
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => (object)new
                {
                    p.PlaylistId,
                    p.Name,
                    SongCount = p.PlaylistSongs.Count()
                })
                .ToList();
        }
    }
}