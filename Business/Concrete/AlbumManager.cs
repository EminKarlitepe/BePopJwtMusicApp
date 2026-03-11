using Business.Abstract;
using Core.Entities;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace Business.Concrete
{
    public class AlbumManager : IAlbumService
    {
        private readonly BepopDbContext _context;

        public AlbumManager(BepopDbContext context)
        {
            _context = context;
        }

        public Album? GetById(int albumId)
        {
            return _context.Albums
                .Include(a => a.Artist)
                .FirstOrDefault(a => a.AlbumId == albumId);
        }

        public List<Album> GetAll()
        {
            return _context.Albums
                .Include(a => a.Artist)
                .ToList();
        }

        public List<Album> GetByArtist(int artistId)
        {
            return _context.Albums
                .Where(a => a.ArtistId == artistId)
                .ToList();
        }

        public List<Album> GetRecent(int take = 8)
        {
            return _context.Albums
                .Include(a => a.Artist)
                .OrderByDescending(a => a.ReleaseDate)
                .Take(take)
                .ToList();
        }

        public void Add(Album album)
        {
            _context.Albums.Add(album);
            _context.SaveChanges();
        }

        public void Update(Album album)
        {
            _context.Albums.Update(album);
            _context.SaveChanges();
        }

        public void Delete(int albumId)
        {
            var album = _context.Albums.FirstOrDefault(a => a.AlbumId == albumId);
            if (album != null)
            {
                _context.Albums.Remove(album);
                _context.SaveChanges();
            }
        }
    }
}