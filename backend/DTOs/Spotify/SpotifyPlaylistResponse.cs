using System.Text.Json.Serialization;

namespace backend.DTOs.Spotify
{
    public class SpotifyPlaylistResponse
    {
        [JsonPropertyName("items")]
        public List<SpotifyPlaylistDTO> Items { get; set; } = new();
        [JsonPropertyName("next")]
        public string? Next { get; set; }
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
    }
}