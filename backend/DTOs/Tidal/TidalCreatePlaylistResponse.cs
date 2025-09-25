using System.Text.Json.Serialization;

namespace backend.DTOs.Tidal

{
    
    public class TidalCreatePlaylistResponse
    {

        [JsonPropertyName("data")]
        public TidalCreatePlaylistData? Data { get; set; }
    }
    
    public class TidalCreatePlaylistData
        {
            [JsonPropertyName("id")]
        public string? Id { get; set; }
        }
 }
