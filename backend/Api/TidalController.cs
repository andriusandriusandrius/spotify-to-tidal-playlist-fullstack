using System.Text.Json;
using backend.DTOs;
using backend.DTOs.Spotify;
using backend.DTOs.Tidal;
using backend.Exceptions;
using backend.Service;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace backend.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TidalController : ControllerBase
    {
        private readonly ITidalService _tidalService;
        private readonly ILogger<TidalController> _logger;
        private readonly IDatabase _redis;

        public TidalController(ITidalService tidalService, ILogger<TidalController> logger, IConnectionMultiplexer redis)
        {
            _tidalService = tidalService;
            _logger = logger;
            _redis = redis.GetDatabase();
        }

        private async Task<ResponseTokenDTO> GetTidalTokens(string state)
        {
            var tokenValue = await _redis.StringGetAsync($"tidal:tokens:{state}");
            if (!tokenValue.HasValue) throw new DomainException("No tokens found for the provided state");

            var tokens = JsonSerializer.Deserialize<ResponseTokenDTO>(tokenValue!)!;
            
            if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken))
                throw new DomainException("Unable to parse tokens");

            return tokens;
        }
        
        [HttpGet("track/{isrc}")]
        public async Task<IActionResult> TidalGetTrackByIsrc(string isrc,string state)
        {
            ResponseTokenDTO tokens = await GetTidalTokens(state);
            string accessToken = tokens.AccessToken!;
            var tracks = await _tidalService.GetTracksByIsrc(isrc, accessToken);

            return Ok(tracks);

        }
        [HttpPost("playlists")]
        public async Task<IActionResult> TidalPostPlaylist(TidalAccessType accessType, string description, string name, string state)
        {
            ResponseTokenDTO tokens = await GetTidalTokens(state);
            string accessToken = tokens.AccessToken!;

            var tracks = await _tidalService.CreatePlaylist(accessType, description, name, accessToken);
            return Ok(tracks);

        }
        [HttpPost("playlists/{playlistId}/track")]
        public async Task<IActionResult> TidalPostTrackToPlaylist(List<string> tidalTracksIds, string playlistId, string state)
        { 
            ResponseTokenDTO tokens = await GetTidalTokens(state);
            string accessToken = tokens.AccessToken!;

            var responseMessage = await _tidalService.AddSongsToPlaylist(tidalTracksIds,playlistId,accessToken);

            return Ok(responseMessage);
            
        }
        
    }
}