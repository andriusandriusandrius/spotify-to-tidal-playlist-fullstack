namespace backend.Configurations
{
    public class TidalAuthOptions
    {
        public string? FrontendRedir { get; set; } = null;
        public string? ClientId { get; set; } = null;
        public string? Redir { get; set; } = null;
        public string[]? Scopes { get; set; } = null;
    }
    public class SpotifyAuthOptions
    {
        public string? FrontendRedir { get; set; } = null;
        public string? ClientId { get; set; } = null;
        public string? Secret { get; set; } = null;
        public string? Redir { get; set; } = null;
        public string[]? Scopes { get; set; } = null;
    }
}