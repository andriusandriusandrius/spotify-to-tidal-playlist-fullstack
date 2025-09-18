using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Unicode;
using System.Web;
using backend.Api;
using backend.DTOs;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
namespace backend.Service
{
    public interface IAuthService
    {
        ApiResponse<string> GenerateState();
        ApiResponse<string> BuildSpotifyAuthLink(string State);
        Task<ApiResponse<SpotifyResponseToken>> SpotifyTokenResponse(string state, string code);
    }
    public class AuthService : IAuthService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;
        private readonly string[] _scopes;
        private readonly HttpClient _http;
        public AuthService(IHttpClientFactory httpFactory)
        {
            Env.Load();
            _clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID") ?? throw new InvalidOperationException("SPOTIFY_CLIENT_ID not defined in env");
            _clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_SECRET") ?? throw new InvalidOperationException("SPOTIFY_SECRET not defined in env");
            _redirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIR") ?? throw new InvalidOperationException("SPOTIFY_REDIR not defined in env");
            _scopes = ["playlist-read-private", "playlist-read-collaborative"];
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
        public ApiResponse<string> BuildSpotifyAuthLink(string state)
        {
            try
            {
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["response.type"] = "code";
                query["client_id"] = _clientId;
                query["scope"] = string.Join(" ", _scopes);
                query["redirect_uri"] = _redirectUri;
                query["state"] = state;

                return new ApiResponse<string> { Success = true, Message = "Spotify auth link succesfully built!", Data = $"https://accounts.spotify.com/authorize?{query}" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}", Data = null };
            }
        }
        public async Task<ApiResponse<SpotifyResponseToken>> SpotifyTokenResponse(string state, string code)
        {
            try
            {
                var header = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
                var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
                request.Headers.Add("Authorization", $"Basic {header}");
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"code",code},
                    {"redirect_uri",_redirectUri},
                    {"grant_type","authorization_code"},
                });
                var response = await _http.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var tokens = JsonSerializer.Deserialize<SpotifyResponseToken>(json);
                return new ApiResponse<SpotifyResponseToken> { Success = true, Message = $"Success! Succesfully got response tokens", Data = tokens };
            }
            catch (Exception ex)
            {
                return new ApiResponse<SpotifyResponseToken> { Success = false, Message = $"Error: {ex.Message}", Data = null };
            }
        }
    }
}  