using System.Text.Json.Serialization;

namespace backend.DTOs.Tidal
{
    public class TidalSearchResponse
    {
        [JsonPropertyName("data")]
        public List<TidalTrackDTO> Data { get; set; } = new();
    }
}