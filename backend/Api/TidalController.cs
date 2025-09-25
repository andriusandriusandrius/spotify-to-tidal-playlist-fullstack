using System.Text.Json;
using backend.DTOs;
using backend.DTOs.Spotify;
using backend.DTOs.Tidal;
using backend.Exceptions;
using backend.Service;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TidalController : ControllerBase
    {
        private readonly ITidalService _tidalService;
        private readonly ILogger<TidalController> _logger;

        public TidalController(ITidalService tidalService, ILogger<TidalController> logger)
        {
            _tidalService = tidalService;
            _logger = logger;
        }
        [HttpGet("track/{isrc}")]
        public async Task<IActionResult> TidalGetTrackByIsrc(string isrc)
        {
            string tokenJson = HttpContext.Session.GetString("TidalTokens") ?? throw new DomainException("No tokens found");
            var tokens = JsonSerializer.Deserialize<ResponseTokenDTO>(tokenJson) ?? throw new DomainException("Unable to parse tokens");
            string accessToken = tokens.AccessToken ?? throw new DomainException("Unable to access parsed tokens");


            var tracks = await _tidalService.GetTracksByIsrc(isrc, accessToken);

            return Ok(tracks);

        }
        [HttpPost("playlists")]
        public async Task<IActionResult> TidalPostPlaylist(TidalAccessType accessType, string description, string name)
        {
            string tokenJson = HttpContext.Session.GetString("TidalTokens") ?? throw new DomainException("No tokens found");
            var tokens = JsonSerializer.Deserialize<ResponseTokenDTO>(tokenJson) ?? throw new DomainException("Unable to parse tokens");
            string accessToken = tokens.AccessToken ?? throw new DomainException("Unable to access parsed tokens");

            var tracks = await _tidalService.CreatePlaylist(accessType, description, name, accessToken);

            return Ok(tracks);

        }
        [HttpPost("playlists/{playlistId}/track")]
        public async Task<IActionResult> TidalPostTrackToPlaylist(List<string> tidalTracksIds, string playlistId)
        { 
            string tokenJson = HttpContext.Session.GetString("TidalTokens") ?? throw new DomainException("No tokens found");
            var tokens = JsonSerializer.Deserialize<ResponseTokenDTO>(tokenJson) ?? throw new DomainException("Unable to parse tokens");
            string accessToken = tokens.AccessToken ?? throw new DomainException("Unable to access parsed tokens");

            var responseMessage = await _tidalService.AddSongsToPlaylist(tidalTracksIds,playlistId,accessToken);

            return Ok(responseMessage);
            
        }
        
    }
}