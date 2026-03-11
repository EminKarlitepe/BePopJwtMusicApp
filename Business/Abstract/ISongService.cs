using Core.Entities;
using Business.DTOs;

namespace Business.Abstract
{
    public interface ISongService
    {
        Song? GetById(int songId);
        List<Song> GetAll();
        List<Song> GetByArtist(int artistId, int excludeSongId = 0, int take = 6);
        List<Song> GetRecentlyAdded(int take = 10);
        List<Song> GetTopPlayed(int take = 6);
        SongPlayResultDto TryPlay(int songId, int userId);
        int GetPlayCount(int songId);
        void Add(Song song, List<int>? genreIds = null);
        void Update(Song song, List<int>? genreIds = null);
        void Delete(int songId);
    }
}