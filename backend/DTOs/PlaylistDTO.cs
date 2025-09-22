namespace backend.DTOs
{
    public class Playlist
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<Track> Tracks = new();
    }
}