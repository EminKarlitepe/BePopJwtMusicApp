using Business.Abstract;
using Core.Entities;
using DataAccess.Context;

namespace Business.Concrete
{
    public class GenreManager : IGenreService
    {
        private readonly BepopDbContext _context;

        public GenreManager(BepopDbContext context)
        {
            _context = context;
        }

        public List<Genre> GetAll()
        {
            return _context.Genres
                .OrderBy(g => g.Name)
                .ToList();
        }

        public Genre? GetById(int genreId)
        {
            return _context.Genres.FirstOrDefault(g => g.GenreId == genreId);
        }

        public void Add(Genre genre)
        {
            _context.Genres.Add(genre);
            _context.SaveChanges();
        }

        public void Update(Genre genre)
        {
            _context.Genres.Update(genre);
            _context.SaveChanges();
        }

        public void Delete(int genreId)
        {
            var genre = _context.Genres.FirstOrDefault(g => g.GenreId == genreId);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
                _context.SaveChanges();
            }
        }
    }
}