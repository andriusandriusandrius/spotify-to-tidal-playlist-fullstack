using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace backend.DTOs.Tidal
{
    public enum TidalAccessType
    {
        [EnumMember(Value="PUBLIC")]
        Public,
        [EnumMember(Value ="PRIVATE")]
        Private
    }
    public class TidalTrackDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        [JsonPropertyName("isrc")]
        public string? Isrc { get; set; }
    }
}