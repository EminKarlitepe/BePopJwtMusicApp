using Core.Entities;

namespace Business.Abstract
{
    public interface IPlaylistService
    {
        List<Playlist> GetByUser(int userId);
        Playlist? GetById(int playlistId, int userId);
        Playlist Create(int userId, string name);
        void Delete(int playlistId, int userId);
        bool AddSong(int playlistId, int songId, int userId);
        bool RemoveSong(int playlistId, int songId, int userId);
        List<object> GetUserPlaylistsSummary(int userId);
    }
}