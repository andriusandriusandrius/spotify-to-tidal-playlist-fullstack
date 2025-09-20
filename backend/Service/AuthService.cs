using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using backend.DTOs;
using DotNetEnv;
namespace backend.Service
{
    public interface IAuthService
    {
        ApiResponse<string> GenerateState();
        ApiResponse<string> BuildSpotifyAuthLink(string State);
        Task<ApiResponse<ResponseToken>> SpotifyTokenResponse(string code);
        ApiResponse<string> GenerateVerifier();
        ApiResponse<string> GenerateCodeChallenge(string verifier);
        ApiResponse<string> BuildTidalAuthLink(string state, string challenge);
        Task<ApiResponse<ResponseToken>> TidalTokenResponse(string code, string verifier);

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

        private readonly HttpClient _http;
        public AuthService(IHttpClientFactory httpFactory)
        {
            Env.Load();
            _spotifyClientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID") ?? throw new InvalidOperationException("SPOTIFY_CLIENT_ID not defined in env");
            _spotifyClientSecret = Environment.GetEnvironmentVariable("SPOTIFY_SECRET") ?? throw new InvalidOperationException("SPOTIFY_SECRET not defined in env");
            _spotifyRedirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIR") ?? throw new InvalidOperationException("SPOTIFY_REDIR not defined in env");
            _spotifyScopes = ["playlist-read-private", "playlist-read-collaborative"];

            _tidalClientId = Environment.GetEnvironmentVariable("TIDAL_CLIENT_ID") ?? throw new InvalidOperationException("TIDAL CLIENT ID not defined in env");
            _tidalRedirectUri = Environment.GetEnvironmentVariable("TIDAL_REDIR") ?? throw new InvalidOperationException("TIDAL_REDIR not defined in env");
            _tidalScopes = ["playlists.write", "playlists.read"];
            _http = httpFactory.CreateClient();
        }
        public ApiResponse<string> GenerateState()
        {
            try
            {
                var stateBytes = new byte[16];
                RandomNumberGenerator.Fill(stateBytes);
                var state = Convert.ToHexString(stateBytes);
                return new ApiResponse<string> { Success = true, Message = "Succesfully generated state", Data = state };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}", Data = null };
            }
        }
        public ApiResponse<string> GenerateVerifier()
        {
            try
            {
                var bytes = new byte[32];
                RandomNumberGenerator.Fill(bytes);
                string verifier = Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
                return new ApiResponse<string> { Success = true, Message = "Succesfully created verifier", Data = verifier };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> { Success = false, Message = $"Error at Generate Verifier: {ex.Message}" };
            }

        }

        public ApiResponse<string> GenerateCodeChallenge(string codeVerifier)
        {
            try
            {
                var sha256 = SHA256.Create();
                var hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier));
                string challenge = Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_").Replace("=", "");
                return new ApiResponse<string> { Success = true, Message = "Succesfully created challenge", Data = challenge };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> { Success = false, Message = $"Error at Generate challenge: {ex.Message}" };
            }
        }
        public ApiResponse<string> BuildSpotifyAuthLink(string state)
        {
            try
            {
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["response_type"] = "code";
                query["client_id"] = _spotifyClientId;
                query["scope"] = string.Join(" ", _spotifyScopes);
                query["redirect_uri"] = _spotifyRedirectUri;
                query["state"] = state;

                return new ApiResponse<string> { Success = true, Message = "Spotify auth link succesfully built!", Data = $"https://accounts.spotify.com/authorize?{query}" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}", Data = null };
            }
        }
        public async Task<ApiResponse<ResponseToken>> SpotifyTokenResponse(string code)
        {
            try
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
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var tokens = JsonSerializer.Deserialize<ResponseToken>(json);
                if (tokens == null) return new ApiResponse<ResponseToken> { Success = false, Message = "Failed to parse spotify tokens!", Data = null };
                return new ApiResponse<ResponseToken> { Success = true, Message = $"Success! Succesfully got response tokens", Data = tokens };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ResponseToken> { Success = false, Message = $"Error: {ex.Message}", Data = null };
            }
        }
        public ApiResponse<string> BuildTidalAuthLink(string state, string challenge)
        {
            try
            {
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["response_type"] = "code";
                query["client_id"] = _tidalClientId;
                query["scope"] = string.Join(" ", _tidalScopes);
                query["redirect_uri"] = _tidalRedirectUri;
                query["code_challenge_method"] = "S256";
                query["code_challenge"] = challenge;
                query["state"] = state;
                return new ApiResponse<string> { Success = true, Message = "Succesfully created tidal auth link!", Data = $"https://login.tidal.com/authorize?{query}" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> { Success = false, Message = $"Failed to create tidal auth link: {ex.Message}", Data = null };
            }

        }
        public async Task<ApiResponse<ResponseToken>> TidalTokenResponse(string code, string verifier)
        {
            try
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
                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<ResponseToken> { Success = false, Message = "Failed to return with a token!" };
                }
                var json = await response.Content.ReadAsStringAsync();
                var tokens = JsonSerializer.Deserialize<ResponseToken>(json);
                return new ApiResponse<ResponseToken> { Success = true, Message = "Token response succeeded!", Data = tokens };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ResponseToken> { Success = false, Message = $"Exception:{ex.Message}" };
            }
        }
    }
}  