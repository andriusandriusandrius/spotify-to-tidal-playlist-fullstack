using System.Security.Cryptography;
using backend.Api;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
namespace backend.Service
{
    public interface IAuthService
    {
        ApiResponse<string> GenerateState();
        Task<ApiResponse<string>> BuildSpotifyAuthLink(string State);
    }
    public class AuthService : IAuthService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;
        private readonly string[] _scopes;
        public AuthService()
        {
            Env.Load();
            _clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID") ?? throw new InvalidOperationException("SPOTIFY_CLIENT_ID not defined in env");
            _clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_SECRET") ?? throw new InvalidOperationException("SPOTIFY_SECRET not defined in env");
            _redirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIR") ?? throw new InvalidOperationException("SPOTIFY_REDIR not defined in env");
            _scopes = ["playlist-read-private", "playlist-read-private"];
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
            catch (Exception ex) {
                return new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}", Data = null };
            }
        }
    }
}  