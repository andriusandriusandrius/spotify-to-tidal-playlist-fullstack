using System.Text.Json.Serialization;

namespace backend.DTOs.Tidal
{
    public class TidalSearchResponse
    {
        [JsonPropertyName("data")]
        public List<TidalTrackIdDTO> Data { get; set; } = new();
    }
}