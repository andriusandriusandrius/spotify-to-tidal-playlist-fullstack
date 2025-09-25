using System.Text.Json.Serialization;

namespace backend.DTOs.Tidal

{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TidalAccessType
    {
        PUBLIC,
        PRIVATE
    }
    public class TidalCreatePlaylistPayload
    {

        [JsonPropertyName("data")]
        public TidalCreatePlaylistPayloadtData? Data { get; set; }
    }
    
    public class TidalCreatePlaylistPayloadtData
        {
            [JsonPropertyName("attributes")]
            public TidalCreatePlaylistPayloadAttributes? Attributes { get; set; }
            
            [JsonPropertyName("type")]
            public string Type { get; set; } = "playlists";
        }
    public class TidalCreatePlaylistPayloadAttributes
        {
            [JsonPropertyName("accessType")]
            public TidalAccessType AccessType { get; set; }
            [JsonPropertyName("description")]
            public string? Description { get; set; }
            [JsonPropertyName("name")]
            public string? Name { get; set; }
        }
 }
