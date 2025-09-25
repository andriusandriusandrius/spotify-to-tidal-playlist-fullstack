using System.Net.Http.Headers;
using System.Text.Json;
using backend.DTOs.Tidal;
using backend.Exceptions;

namespace backend.Service
{
    public interface ITidalService
    {
        Task<List<TidalTrackIdDTO>> GetTracksByIsrc(string isrc, string accessToken);
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
            var trackResponse = JsonSerializer.Deserialize<TidalSearchResponse>(jsonContent) ??throw new JsonException($"Failed to deserialize playlist failed ({response.StatusCode})"); ;

            List<TidalTrackIdDTO> tracks = trackResponse.Data;

            return tracks;
        }
       
    }
}