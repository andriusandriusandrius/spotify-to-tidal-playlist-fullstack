using System.Text.Json.Serialization;

namespace backend.DTOs.Tidal
{
    public class TidalInsertTracksToPlaylistPayload
    {
        [JsonPropertyName("data")]
        public List<TidalInsertTrackToPlaylistData> Data { get; set; } = new();
    }
    public class TidalInsertTrackToPlaylistData
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; } = "tracks";
    }
}