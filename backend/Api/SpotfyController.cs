using System.Text.Json;
using backend.DTOs;
using backend.DTOs.Spotify;
using backend.Exceptions;
using backend.Service;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpotifyController : ControllerBase
    {
        private readonly ISpotifyService _spotifyService;
        private readonly ILogger<SpotifyController> _logger;

        public SpotifyController(ISpotifyService spotifyService, ILogger<SpotifyController> logger)
        {
            _spotifyService = spotifyService;
            _logger = logger;
        }
        [HttpGet("playlist/{playlistId}")]
        public async Task<IActionResult> SpotifyGetPlaylist(string playlistId)
        {
            string tokenJson = HttpContext.Session.GetString("SpotifyTokens") ?? throw new DomainException("No tokens found");
            var tokens = JsonSerializer.Deserialize<ResponseTokenDTO>(tokenJson) ?? throw new DomainException("Unable to parse tokens");
            string accessToken = tokens.AccessToken ?? throw new DomainException("Unable to access parsed tokens");


            SpotifyPlaylistDTO spotifyPlaylist = await _spotifyService.GetPlaylist(playlistId, accessToken);

            return Ok(spotifyPlaylist);

        }
        [HttpGet("playlist/{playlistId}/tracks")]
        public async Task<IActionResult> SpotifyGetPlaylistTracks(string playlistId)
        {
            string tokenJson = HttpContext.Session.GetString("SpotifyTokens") ?? throw new DomainException("No tokens found");
            var tokens = JsonSerializer.Deserialize<ResponseTokenDTO>(tokenJson) ?? throw new DomainException("Unable to parse tokens");
            string accessToken = tokens.AccessToken ?? throw new DomainException("Unable to access parsed tokens");

            List<SpotifyTrackDTO> tracks = await _spotifyService.GetPlaylistTracks(playlistId, accessToken);
            return Ok(tracks);
        }
        [HttpGet("playlist")]
        public async Task<IActionResult> SpotifyGetUserPlaylists()
        {
            string tokenJson = HttpContext.Session.GetString("SpotifyTokens") ?? throw new DomainException("No tokens found");
            var tokens = JsonSerializer.Deserialize<ResponseTokenDTO>(tokenJson) ?? throw new DomainException("Unable to parse tokens");
            string accessToken = tokens.AccessToken ?? throw new DomainException("Unable to access parsed tokens");

            List<SpotifyPlaylistDTO> tracks = await _spotifyService.GetUserPlaylists(accessToken);
            return Ok(tracks);
        }

    }
}