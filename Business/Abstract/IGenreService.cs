using Core.Entities;

namespace Business.Abstract
{
    public interface IGenreService
    {
        List<Genre> GetAll();
        Genre? GetById(int genreId);
        void Add(Genre genre);
        void Update(Genre genre);
        void Delete(int genreId);
    }
}