using System.Text.Json.Serialization;

namespace backend.DTOs.Tidal

{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TidalAccessType
    {
        Public,
        Private
    }
    public class TidalCreatePlaylistPayload
    {

        [JsonPropertyName("data")]
        public TidalCreatePlaylistData? Data { get; set; }
    }
    
    public class TidalCreatePlaylistData
        {
            [JsonPropertyName("attributes")]
            public TidalCreatePlaylistAttributes? Attributes { get; set; }
            public string Type = "playlists";
        }
    public class TidalCreatePlaylistAttributes
        {
            [JsonPropertyName("accesstype")]
            public string? AccessType { get; set; }
            [JsonPropertyName("description")]
            public string? Description { get; set; }
            [JsonPropertyName("name")]
            public string? Name { get; set; }
        }
 }
