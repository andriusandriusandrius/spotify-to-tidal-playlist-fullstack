namespace backend.DTOs {
    public class TrackDTO
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Artist { get; set; }
        public string? Album { get; set; }
        public string? Isrc { get; set; }
        public int DurationSeconds { get; set; }
        public string? Source { get; set; }
    }
}