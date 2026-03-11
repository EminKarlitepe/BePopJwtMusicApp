using Core.Entities;

namespace Business.Abstract
{
    public interface IArtistService
    {
        Artist? GetById(int artistId);
        List<Artist> GetAll(string? country = null);
        List<string?> GetCountries();
        int GetFollowersCount(int artistId);
        bool IsFollowing(int artistId, int userId);
        void ToggleFollow(int artistId, int userId);
        void Add(Artist artist);
        void Update(Artist artist);
        void Delete(int artistId);
    }
}