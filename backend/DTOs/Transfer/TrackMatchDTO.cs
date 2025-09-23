using backend.DTOs.Spotify;
using backend.DTOs.Tidal;

namespace backend.DTOs.Transfer
{ 
    public class TrackMatchDTO
    {
        public SpotifyTrackDTO? SpotifyTrack { get; set; }
        public TidalTrackDTO? TidalTrack { get; set; }
        public bool IsMatched { get; set; }
        public double MatchScore { get; set; }
        public string? MatchMethod { get; set; }
    }
}