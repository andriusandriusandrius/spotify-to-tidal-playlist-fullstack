using System.Text.Json;
using backend.DTOs;
using backend.Exceptions;
using backend.Service;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
namespace backend.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly string _frontendUrl;
        public AuthController(IAuthService authService)
        {
            Env.Load();
            _authService = authService;
            _frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_REDIR") ?? throw new InvalidOperationException("FRONTEND_REDIR not defined in env");
        }
        [HttpGet("spotify/login")]
        public IActionResult SpotifyLogin()
        {

            string generateStateResponse = _authService.GenerateState();

            HttpContext.Session.SetString("SpotifyAuthState", generateStateResponse);

            string authLinkResponse = _authService.BuildSpotifyAuthLink(generateStateResponse);

            return Redirect(authLinkResponse);
        }

        [HttpGet("spotify/callback")]
        public async Task<IActionResult> SpotifyCallback([FromQuery] string code, [FromQuery] string state)
        {
            var sessionState = HttpContext.Session.GetString("SpotifyAuthState");
            if (sessionState == null || sessionState != state)
            {
                throw new StateException ("Invalid Spotify state");
            }
            ResponseTokenDTO tokensResponse = await _authService.SpotifyTokenResponse(code);
            HttpContext.Session.SetString("SpotifyTokens", JsonSerializer.Serialize(tokensResponse));

            return Redirect($"{_frontendUrl}login/success?state={state}");
        }
        [HttpGet("spotify/tokens")]
        public IActionResult SpotifyGetTokens([FromQuery] string state)
        {
            var sessionState = HttpContext.Session.GetString("SpotifyAuthState");
            if (sessionState == null || state != sessionState)
            {
               throw new StateException ("Invalid Spotify state");
            }
            var tokens = HttpContext.Session.GetString("SpotifyTokens");
            if (tokens == null) return NotFound("No tokens found");

            return Ok(tokens);
        }

        [HttpGet("tidal/login")]
        public IActionResult TidalLogin()
        {
            string stateResponse = _authService.GenerateState();
            HttpContext.Session.SetString("TidalAuthState", stateResponse);

            string verifierResponse = _authService.GenerateVerifier();
            HttpContext.Session.SetString("TidalVerifier", verifierResponse);

            string challengeResponse = _authService.GenerateCodeChallenge(verifierResponse);
            HttpContext.Session.SetString("TidalChallenge", challengeResponse);

            string authLinkResponse = _authService.BuildTidalAuthLink(stateResponse, challengeResponse);
            return Redirect(authLinkResponse);


        }
        
        [HttpGet("tidal/callback")]
        public async Task<IActionResult> TidalCallback([FromQuery] string code, [FromQuery] string state)
        {
            var sessionState = HttpContext.Session.GetString("TidalAuthState");
            if (sessionState == null || sessionState != state)
            {
                throw new StateException("Invalid State");
            }
            var verifier = HttpContext.Session.GetString("TidalVerifier");
            if (verifier == null)
            {
                return BadRequest("Invalid verifier");
            }
            ResponseTokenDTO tokensResponse = await _authService.TidalTokenResponse(code,verifier);
            HttpContext.Session.SetString("TidalTokens", JsonSerializer.Serialize(tokensResponse));

            return Redirect($"{_frontendUrl}login/success?state={state}");
        }
        [HttpGet("tidal/tokens")]
        public async Task<IActionResult> TidalGetTokens([FromQuery] string state)
        {
            var sessionState = HttpContext.Session.GetString("TidalAuthState");
            if (sessionState == null || state != sessionState)
            {
                throw new StateException("Invalid Tidal State");
            }
            var tokens = HttpContext.Session.GetString("TidalTokens");
            if (tokens == null) return NotFound("No tokens found");

            return Ok(tokens);
        }
    }
    
}