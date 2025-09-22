namespace backend.DTOs.Transfer
{ 
    public class PlaylistTransferResultDTO
    {
        public string? TidalPlaylistId { get; set; }
        public string? TidalPlaylistName { get; set; }
        public int TotalSpotifyTracks { get; set; }
        public int MatchedTracks { get; set; }
        public List<TrackMatchDTO> TrackMatches { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public bool Success { get; set; }
    }
}