using System.Text.Json.Serialization;

namespace backend.DTOs.Spotify
{
    public class SpotifyImageDTO
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }
        [JsonPropertyName("height")]
        public int? Height { get; set; }
        [JsonPropertyName("width")]
        public int? Width { get; set; }
    }
}