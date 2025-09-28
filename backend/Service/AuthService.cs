using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using backend.Configurations;
using backend.DTOs;
using backend.Exceptions;
using Microsoft.Extensions.Options;
namespace backend.Service
{
    public interface IAuthService
    {
        string GenerateState();
        string BuildSpotifyAuthLink(string State);
        Task<ResponseTokenDTO> SpotifyTokenResponse(string code);
        string GenerateVerifier();
        string GenerateCodeChallenge(string verifier);
        string BuildTidalAuthLink(string state, string challenge);
        Task<ResponseTokenDTO> TidalTokenResponse(string code, string verifier);

    }
    public class AuthService : IAuthService
    {
        private readonly string _spotifyClientId;
        private readonly string _spotifyClientSecret;
        private readonly string _spotifyRedirectUri;
        private readonly string[] _spotifyScopes;
        private readonly string _tidalClientId;
        private readonly string _tidalRedirectUri;
        private readonly string[] _tidalScopes;

        private readonly ILogger<AuthService> _logger;

        private readonly HttpClient _http;
        public AuthService(IHttpClientFactory httpFactory, ILogger<AuthService> logger, IOptions<SpotifyAuthOptions> spotifyOptions, IOptions<TidalAuthOptions> tidalOptions)
        {
            _spotifyClientId = spotifyOptions.Value.ClientId?? throw new InvalidOperationException("SPOTIFY_CLIENT_ID not defined");
            _spotifyClientSecret =  spotifyOptions.Value.Secret?? throw new InvalidOperationException("SPOTIFY_SECRET not defined");
            _spotifyRedirectUri = spotifyOptions.Value.Redir ?? throw new InvalidOperationException("SPOTIFY_REDIR not defined");
            _spotifyScopes = spotifyOptions.Value.Scopes ?? throw new InvalidOperationException("SPOTIFY_SCOPES not defined");

            _tidalClientId = tidalOptions.Value.ClientId ?? throw new InvalidOperationException("TIDAL CLIENT ID not defined");
            _tidalRedirectUri = tidalOptions.Value.Redir ?? throw new InvalidOperationException("TIDAL_REDIR not defined");
            _tidalScopes = tidalOptions.Value.Scopes ?? throw new InvalidOperationException("TIDAL_SCOPES not defined");
            _http = httpFactory.CreateClient();
            _logger = logger;
        }
        public string GenerateState()
        {
            var stateBytes = new byte[16];
            RandomNumberGenerator.Fill(stateBytes);
            var state = Convert.ToHexString(stateBytes);
            return  state ;
          
        }
        public string GenerateVerifier()
        {
            var bytes = new byte[32];
            RandomNumberGenerator.Fill(bytes);
            string verifier = Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
            return  verifier ;
        }

        public string GenerateCodeChallenge(string codeVerifier)
        {
            var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier));
            string challenge = Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_").Replace("=", "");
            return challenge;
        }
        public string BuildSpotifyAuthLink(string state)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["response_type"] = "code";
            query["client_id"] = _spotifyClientId;
            query["scope"] = string.Join(" ", _spotifyScopes);
            query["redirect_uri"] = _spotifyRedirectUri;
            query["state"] = state;

            return $"https://accounts.spotify.com/authorize?{query}";    
        }
        public async Task<ResponseTokenDTO> SpotifyTokenResponse(string code)
        {
            var header = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_spotifyClientId}:{_spotifyClientSecret}"));
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            request.Headers.Add("Authorization", $"Basic {header}");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"code",code},
                {"redirect_uri",_spotifyRedirectUri},
                {"grant_type","authorization_code"},
            });
            var response = await _http.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Spotify token return endpoint returned: {Status}: {Body}", response.StatusCode, body);
                throw new ExternalServiceException($"Spotify token response failed ({response.StatusCode})");
                
            }
            var tokens = JsonSerializer.Deserialize<ResponseTokenDTO>(body) ?? throw new ExternalServiceException($"Failed to parse Spotify token response");

            return tokens;
        }
        public string BuildTidalAuthLink(string state, string challenge)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            
            query["response_type"] = "code";
            query["client_id"] = _tidalClientId;
            query["scope"] = string.Join(" ", _tidalScopes);
            query["redirect_uri"] = _tidalRedirectUri;
            query["code_challenge_method"] = "S256";
            query["code_challenge"] = challenge;
            query["state"] = state;

            return $"https://login.tidal.com/authorize?{query}";

        }
        public async Task<ResponseTokenDTO> TidalTokenResponse(string code, string verifier)
        {
            Dictionary<string, string> parameters = new(){
                {"grant_type","authorization_code"},
                { "client_id",_tidalClientId},
                { "code",code},
                { "redirect_uri",_tidalRedirectUri},
                { "code_verifier",verifier}

            };
            var content = new FormUrlEncodedContent(parameters);
            var response = await _http.PostAsync("https://auth.tidal.com/v1/oauth2/token", content);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Tidal token return endpoint returned: {Status}: {Body}", response.StatusCode, body);
                throw new ExternalServiceException($"Tidal token response failed ({response.StatusCode})");
                
            }

            var tokens = JsonSerializer.Deserialize<ResponseTokenDTO>(body) ?? throw new ExternalServiceException("Failed to parse Tidal token response ");
            return  tokens ;
        }
        
    }
}  