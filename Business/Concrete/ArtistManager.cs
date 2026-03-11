using Business.Abstract;
using Core.Entities;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace Business.Concrete
{
    public class ArtistManager : IArtistService
    {
        private readonly BepopDbContext _context;

        public ArtistManager(BepopDbContext context)
        {
            _context = context;
        }

        public Artist? GetById(int artistId)
        {
            return _context.Artists
                .Include(a => a.Songs).ThenInclude(s => s.Album)
                .FirstOrDefault(a => a.ArtistId == artistId);
        }

        public List<Artist> GetAll(string? country = null)
        {
            var query = _context.Artists.AsQueryable();
            if (!string.IsNullOrEmpty(country) && country != "All countries")
                query = query.Where(a => a.Country == country);
            return query.ToList();
        }

        public List<string?> GetCountries()
        {
            return _context.Artists
                .Select(a => a.Country)
                .Distinct()
                .ToList();
        }

        public int GetFollowersCount(int artistId)
        {
            return _context.ArtistFollows.Count(af => af.ArtistId == artistId);
        }

        public bool IsFollowing(int artistId, int userId)
        {
            return userId > 0 && _context.ArtistFollows.Any(af => af.ArtistId == artistId && af.UserId == userId);
        }

        public void ToggleFollow(int artistId, int userId)
        {
            var existing = _context.ArtistFollows.FirstOrDefault(af => af.ArtistId == artistId && af.UserId == userId);
            if (existing == null)
                _context.ArtistFollows.Add(new ArtistFollow { ArtistId = artistId, UserId = userId });
            else
                _context.ArtistFollows.Remove(existing);
            _context.SaveChanges();
        }

        public void Add(Artist artist)
        {
            _context.Artists.Add(artist);
            _context.SaveChanges();
        }

        public void Update(Artist artist)
        {
            _context.Artists.Update(artist);
            _context.SaveChanges();
        }

        public void Delete(int artistId)
        {
            var artist = _context.Artists.FirstOrDefault(a => a.ArtistId == artistId);
            if (artist != null)
            {
                _context.Artists.Remove(artist);
                _context.SaveChanges();
            }
        }
    }
}