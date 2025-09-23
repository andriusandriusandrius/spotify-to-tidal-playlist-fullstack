using System.Text.Json.Serialization;

namespace backend.DTOs.Spotify
{
    public class SpotifyArtistDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}