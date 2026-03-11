using Business.Abstract;
using Business.DTOs;
using Core.Entities;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace Business.Concrete
{
    public class SongManager : ISongService
    {
        private readonly BepopDbContext _context;

        public SongManager(BepopDbContext context)
        {
            _context = context;
        }

        public Song? GetById(int songId)
        {
            return _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Album).ThenInclude(a => a!.Songs)
                .Include(s => s.SongGenres).ThenInclude(sg => sg.Genre)
                .FirstOrDefault(s => s.SongId == songId);
        }

        public List<Song> GetAll()
        {
            return _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Album)
                .OrderByDescending(s => s.CreatedAt)
                .ToList();
        }

        public List<Song> GetByArtist(int artistId, int excludeSongId = 0, int take = 6)
        {
            return _context.Songs
                .Include(s => s.Album)
                .Where(s => s.ArtistId == artistId && s.SongId != excludeSongId)
                .OrderByDescending(s => s.CreatedAt)
                .Take(take)
                .ToList();
        }

        public List<Song> GetRecentlyAdded(int take = 10)
        {
            return _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Album)
                .OrderByDescending(s => s.CreatedAt)
                .Take(take)
                .ToList();
        }

        public List<Song> GetTopPlayed(int take = 6)
        {
            return _context.PlayHistories
                .Include(p => p.Song).ThenInclude(s => s.Artist)
                .Include(p => p.Song).ThenInclude(s => s.Album)
                .GroupBy(p => p.Song)
                .OrderByDescending(g => g.Count())
                .Take(take)
                .Select(g => g.Key)
                .ToList();
        }

        public SongPlayResultDto TryPlay(int songId, int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            var userLevel = user?.MembershipLevel ?? 0;

            var song = _context.Songs.FirstOrDefault(s => s.SongId == songId);
            if (song == null)
                return new SongPlayResultDto { CanPlay = false, ErrorMessage = "Şarkı bulunamadı." };

            if (userLevel < song.Level)
                return new SongPlayResultDto { CanPlay = false, ErrorMessage = "Bu şarkıyı dinlemek için paket yükseltmeniz gerekiyor." };

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

            return new SongPlayResultDto { CanPlay = true, FileUrl = song.FileUrl };
        }

        public int GetPlayCount(int songId)
        {
            return _context.PlayHistories.Count(p => p.SongId == songId);
        }

        public void Add(Song song, List<int>? genreIds = null)
        {
            _context.Songs.Add(song);
            _context.SaveChanges();

            if (genreIds != null)
            {
                foreach (var gid in genreIds.Distinct())
                    _context.SongGenres.Add(new SongGenre { SongId = song.SongId, GenreId = gid });
                _context.SaveChanges();
            }
        }

        public void Update(Song song, List<int>? genreIds = null)
        {
            _context.Songs.Update(song);

            if (genreIds != null)
            {
                var old = _context.SongGenres.Where(sg => sg.SongId == song.SongId);
                _context.SongGenres.RemoveRange(old);
                foreach (var gid in genreIds.Distinct())
                    _context.SongGenres.Add(new SongGenre { SongId = song.SongId, GenreId = gid });
            }

            _context.SaveChanges();
        }

        public void Delete(int songId)
        {
            var song = _context.Songs.FirstOrDefault(s => s.SongId == songId);
            if (song != null)
            {
                _context.Songs.Remove(song);
                _context.SaveChanges();
            }
        }
    }
}