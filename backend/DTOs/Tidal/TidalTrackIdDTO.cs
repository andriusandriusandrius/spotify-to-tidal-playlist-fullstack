using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace backend.DTOs.Tidal
{

    public class TidalTrackIdDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }
}