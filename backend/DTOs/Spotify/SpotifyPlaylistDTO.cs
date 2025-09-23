using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace backend.DTOs.Spotify
{
    public class SpotifyPlaylistDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("images")]
        public List<SpotifyImageDTO>? Images { get; set; } = new();
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("public")]
        public bool Public { get; set; }
        [JsonPropertyName("tracks")]
        public SpotifyPlaylistTracksDTO Tracks { get; set; } = new();
    }
}