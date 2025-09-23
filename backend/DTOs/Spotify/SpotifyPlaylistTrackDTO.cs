using System.Text.Json.Serialization;

namespace backend.DTOs.Spotify
{
    public class SpotifyPlaylistTrackDTO
    {
        [JsonPropertyName("track")]
        public SpotifyTrackDTO? Track { get; set; }
    }
}