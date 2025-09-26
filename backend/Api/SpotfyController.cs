using System.Text.Json;
using backend.DTOs;
using backend.DTOs.Spotify;
using backend.Exceptions;
using backend.Service;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace backend.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpotifyController : ControllerBase
    {
        private readonly ISpotifyService _spotifyService;
        private readonly ILogger<SpotifyController> _logger;
        private readonly IDatabase _redis;

        public SpotifyController(ISpotifyService spotifyService, ILogger<SpotifyController> logger, IConnectionMultiplexer redis)
        {
            _spotifyService = spotifyService;
            _logger = logger;
            _redis = redis.GetDatabase();
        }
        private async Task<ResponseTokenDTO> GetSpotifyTokens(string state)
        {
            var tokenValue = await _redis.StringGetAsync($"spotify:tokens:{state}");
            if (!tokenValue.HasValue) throw new DomainException("No tokens found for the provided state");

            var tokens = JsonSerializer.Deserialize<ResponseTokenDTO>(tokenValue!)!;
            if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken))
                throw new DomainException("Unable to parse tokens");

            return tokens;
        }
        [HttpGet("playlist/{playlistId}")]
        public async Task<IActionResult> SpotifyGetPlaylist(string playlistId, string state)
        {
            ResponseTokenDTO tokens = await GetSpotifyTokens(state);
            string accessToken = tokens.AccessToken!;

            SpotifyPlaylistDTO spotifyPlaylist = await _spotifyService.GetPlaylist(playlistId, accessToken);

            return Ok(spotifyPlaylist);

        }
        [HttpGet("playlist/{playlistId}/tracks")]
        public async Task<IActionResult> SpotifyGetPlaylistTracks(string playlistId, string state)
        {
             ResponseTokenDTO tokens = await GetSpotifyTokens(state);
            string accessToken = tokens.AccessToken!;

            List<SpotifyTrackDTO> tracks = await _spotifyService.GetPlaylistTracks(playlistId, accessToken);
            return Ok(tracks);
        }
        [HttpGet("playlist")]
        public async Task<IActionResult> SpotifyGetUserPlaylists(string state)
        {
             ResponseTokenDTO tokens = await GetSpotifyTokens(state);
            string accessToken = tokens.AccessToken!;

            List<SpotifyPlaylistDTO> tracks = await _spotifyService.GetUserPlaylists(accessToken);
            return Ok(tracks);
        }

    }
}