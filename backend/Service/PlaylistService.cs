using backend.DTOs;

namespace backend.Service
{
    public interface IPlaylistService {
        Task<List<Playlist>> SpotifyGetPlaylists(string accessToken);
    }
    public class PlaylistService : IPlaylistService
    { 
        public
    }
}