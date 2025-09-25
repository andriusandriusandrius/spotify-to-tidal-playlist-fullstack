using backend.DTOs.Tidal;

namespace backend.DTOs.Transfer
{
    public class PlaylistTransferDTO
    {
        public string? SpotifyPlaylistId { get; set; }
        public string? TidalPlaylistName { get; set; }
        public string? TidalPlaylistDescription { get; set; }
        public TidalAccessType AccessType { get; set; } = TidalAccessType.PRIVATE;
        public bool IncludeUnmatched { get; set; } = false;
    }
}