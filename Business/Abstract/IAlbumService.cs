using Core.Entities;

namespace Business.Abstract
{
    public interface IAlbumService
    {
        Album? GetById(int albumId);
        List<Album> GetAll();
        List<Album> GetByArtist(int artistId);
        List<Album> GetRecent(int take = 8);
        void Add(Album album);
        void Update(Album album);
        void Delete(int albumId);
    }
}