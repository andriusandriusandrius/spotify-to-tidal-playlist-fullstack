using System.Text.Json.Serialization;

namespace backend.DTOs.Spotify
{
    public class SpotifyTrackDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("artists")]
        public List<SpotifyArtistDTO>? Artists { get; set; } = new();
        [JsonPropertyName("album")]
        public SpotifyAlbumDTO? Album { get; set; }
        [JsonPropertyName("external_ids")]
        public SpotifyTrackExternalIdsDTO? ExternalIds { get; set; }

    }
    public class SpotifyTrackExternalIdsDTO
    {
        [JsonPropertyName("isrc")]
        public string? Isrc { get; set; }
    }
}