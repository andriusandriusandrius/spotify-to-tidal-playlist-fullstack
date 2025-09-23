using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace backend.DTOs.Spotify
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ReleaseDatePrecision
    {
        Year,
        Month,
        Day
    }
    public class SpotifyAlbumDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("album_type")]
        public string? AlbumType { get; set; }
        [JsonPropertyName("total_tracks")]
        public int TotalTracks { get; set; }
        [JsonPropertyName("images")]
        public List<SpotifyImageDTO>? Images { get; set; } = new();
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }
        [JsonPropertyName("release_date_precision")]
        public ReleaseDatePrecision? ReleaseDatePrecision { get; set; }
    }
}