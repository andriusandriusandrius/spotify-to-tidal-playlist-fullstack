using System.Text.Json.Serialization;

namespace backend.DTOs.Spotify
{
    public class SpotifyPlaylistTrackResponse
    {
        [JsonPropertyName("items")]
        public List<SpotifyPlaylistTrackDTO> Items { get; set; } = new();
        [JsonPropertyName("next")]
        public string? Next { get; set; }
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
    }
}