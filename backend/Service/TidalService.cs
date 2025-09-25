using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using backend.DTOs.Tidal;
using backend.Exceptions;

namespace backend.Service
{
    public interface ITidalService
    {
        Task<List<TidalTrackIdDTO>> GetTracksByIsrc(string isrc, string accessToken);
        Task<TidalCreatePlaylistResponse> CreatePlaylist(TidalAccessType accessType, string description, string name, string accessToken);
        Task<string> AddSongsToPlaylist(List<string> tidalTracksIds, string playlistId, string accessToken);
    }
    public class TidalService : ITidalService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TidalService> _logger;
        private const string BaseUrl = "https://openapi.tidal.com/v2/";
        public TidalService(HttpClient httpClient, ILogger<TidalService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }
        public async Task<List<TidalTrackIdDTO>> GetTracksByIsrc(string isrc, string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync($"tracks?countryCode=US&filter%5Bisrc%5D={isrc}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("GetTrackByIsrc endpoint returned: {Status}", response.StatusCode);
                throw new ExternalServiceException($"Get track by isrc failed ({response.StatusCode})");
            }
            var jsonContent = await response.Content.ReadAsStringAsync();
            var trackResponse = JsonSerializer.Deserialize<TidalSearchResponse>(jsonContent) ?? throw new JsonException($"Failed to deserialize playlist failed ({response.StatusCode})"); ;

            List<TidalTrackIdDTO> tracks = trackResponse.Data;

            return tracks;
        }
        public async Task<TidalCreatePlaylistResponse> CreatePlaylist(TidalAccessType accessType, string description, string name, string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var payload = new TidalCreatePlaylistPayload
            {
                Data = new TidalCreatePlaylistPayloadtData
                {
                    Attributes = new()
                    {
                        AccessType = accessType,
                        Description = description,
                        Name = name
                    }
                }
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/vnd.api+json");

            var response = await _httpClient.PostAsync("playlists?countryCode=US", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("CreatePlaylist endpoint returned: {Status}", response.StatusCode);
                throw new ExternalServiceException($"Create playlist failed ({response.StatusCode})");
            }


            var responseJson = await response.Content.ReadAsStringAsync();
            var playlistResponse = JsonSerializer.Deserialize<TidalCreatePlaylistResponse>(responseJson) ?? throw new JsonException("Failed to deserialize create playlist response");

            return playlistResponse;
        }
        public async Task<string> AddSongsToPlaylist(List<string> tidalTracksIds, string playlistId, string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


            var payload = new TidalInsertTracksToPlaylistPayload
            {
                Data = [.. tidalTracksIds.Select(id => new TidalInsertTrackToPlaylistData { Id = id })]
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/vnd.api+json");

            var response = await _httpClient.PostAsync($"playlists/{playlistId}/relationships/items?countryCode=US", content);
            var responseJson = await response.Content.ReadAsStringAsync();

            return responseJson;
        }
       
    }
}