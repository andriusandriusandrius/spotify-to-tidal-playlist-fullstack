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
        
    }
}