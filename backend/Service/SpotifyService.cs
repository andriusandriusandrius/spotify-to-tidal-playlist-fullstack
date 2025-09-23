using System.Net.Http.Headers;
using System.Text.Json;
using backend.DTOs.Spotify;
using backend.Exceptions;

namespace backend.Service
{
    public interface ISpotifyService
    {
        Task<SpotifyPlaylistDTO> GetPlaylist(string playlistId, string accessToken);
        Task<List<SpotifyTrackDTO>> GetPlaylistTracks(string playlistId, string accessToken);
        Task<List<SpotifyPlaylistDTO>> GetUserPlaylists(string accessToken);

    }
    public class SpotifyService : ISpotifyService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SpotifyService> _logger;
        private const string BaseUrl = "https://api.spotify.com/v1/";
        public SpotifyService(HttpClient httpClient, ILogger<SpotifyService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }
        public async Task<SpotifyPlaylistDTO> GetPlaylist(string playlistId, string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync($"playlists/{playlistId}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("GetPlaylist endpoint returned: {Status}", response.StatusCode);
                throw new ExternalServiceException($"Get playlist by id failed ({response.StatusCode})");
            }
            var jsonContent = await response.Content.ReadAsStringAsync();
            var playlist = JsonSerializer.Deserialize<SpotifyPlaylistDTO>(jsonContent);
            if (playlist == null)
            {
                _logger.LogError("GetPlaylist endpoint returned: {Status}", response.StatusCode);
                throw new JsonException($"Failed to deserialize playlist failed ({response.StatusCode})");
            }
            return playlist;
        }
        public async Task<List<SpotifyTrackDTO>> GetPlaylistTracks(string playlistId, string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            List<SpotifyTrackDTO> allTracks = new();
            var offset = 0;
            const int limit = 50;
            bool areMore = true;
            while (areMore)
            {
                var response = await _httpClient.GetAsync($"playlists/{playlistId}/tracks?offset={offset}&limit={limit}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Get playlist tracks endpoint returned: {Status}", response.StatusCode);
                    throw new ExternalServiceException($"Get playlist tracks failed ({response.StatusCode})");
                }
                var jsonContent = await response.Content.ReadAsStringAsync();
                var tracksResponse = JsonSerializer.Deserialize<SpotifyPlaylistTrackResponse>(jsonContent);

                if (tracksResponse?.Items != null)
                {
                    List<SpotifyTrackDTO> tracks = tracksResponse.Items.Where(item => item.Track != null).Select(item => item.Track!).ToList();
                    allTracks.AddRange(tracks);
                }
                areMore = tracksResponse?.Next != null;
                offset = offset + limit;
                await Task.Delay(100);
            }
            _logger.LogInformation("{trackCount} tracks retrieved from Spotify playlist {playlistId}", allTracks.Count, playlistId);
            return allTracks;
        }
        public async Task<List<SpotifyPlaylistDTO>> GetUserPlaylists(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            List<SpotifyPlaylistDTO> allPlaylists = new();

            var offset = 0;
            const int limit = 50;
            bool areMore = true;
            while (areMore)
            {
                var response = await _httpClient.GetAsync($"me/playlists?offset={offset}&limit={limit}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Get playlists from user endpoint returned: {Status}", response.StatusCode);
                    throw new ExternalServiceException($"Get playlists failed ({response.StatusCode})");
                }
                var jsonContent = await response.Content.ReadAsStringAsync();
                var playlistsResponse = JsonSerializer.Deserialize<SpotifyPlaylistResponse>(jsonContent);
                if (playlistsResponse?.Items != null)
                {
                    List<SpotifyPlaylistDTO> playlists = playlistsResponse.Items;
                    allPlaylists.AddRange(playlists);
                }
                areMore = playlistsResponse?.Next != null;
                offset += limit;
                await Task.Delay(100);
            }
            _logger.LogInformation("{playlistCount} tracks retrieved from Spotify user", allPlaylists.Count);
            return allPlaylists;
        }
    }
}