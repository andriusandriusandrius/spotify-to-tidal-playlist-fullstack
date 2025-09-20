using System.Text.Json;
using backend.DTOs;
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

            ApiResponse<string> generateStateResponse = _authService.GenerateState();
            if (!generateStateResponse.Success) return BadRequest(generateStateResponse.Message);

            HttpContext.Session.SetString("SpotifyAuthState", generateStateResponse.Data);

            ApiResponse<string> authLinkResponse = _authService.BuildSpotifyAuthLink(generateStateResponse.Data);
            if (!authLinkResponse.Success) return BadRequest(authLinkResponse.Message);

            return Redirect(authLinkResponse.Data);
        }

        [HttpGet("spotify/callback")]
        public async Task<IActionResult> SpotifyCallback([FromQuery] string code, [FromQuery] string state)
        {
            var sessionState = HttpContext.Session.GetString("SpotifyAuthState");
            if (sessionState == null || sessionState != state)
            {
                return BadRequest("Invalid State");
            }
            ApiResponse<ResponseToken> tokensResponse = await _authService.SpotifyTokenResponse(code);
            HttpContext.Session.SetString("SpotifyTokens", JsonSerializer.Serialize(tokensResponse.Data));

            return Redirect($"{_frontendUrl}login/success?state={state}");
        }
        [HttpGet("spotify/tokens")]
        public IActionResult SpotifyGetTokens([FromQuery] string state)
        {
            var sessionState = HttpContext.Session.GetString("SpotifyAuthState");
            if (sessionState == null || state != sessionState)
            {
                return BadRequest("Invalid State");
            }
            var tokens = HttpContext.Session.GetString("SpotifyTokens");
            if (tokens == null) return BadRequest("No tokens found");

            return Ok(tokens);
        }

        [HttpGet("tidal/login")]
        public IActionResult TidalLogin()
        {
            ApiResponse<string> stateResponse = _authService.GenerateState();
            if (!stateResponse.Success) return BadRequest(stateResponse.Message);
            HttpContext.Session.SetString("TidalAuthState", stateResponse.Data);

            ApiResponse<string> verifierResponse = _authService.GenerateVerifier();
            if (!verifierResponse.Success) return BadRequest(verifierResponse.Message);
            HttpContext.Session.SetString("TidalVerifier", verifierResponse.Data);

            ApiResponse<string> challengeResponse = _authService.GenerateCodeChallenge(verifierResponse.Data);
            if (!challengeResponse.Success) return BadRequest(challengeResponse.Message);
            HttpContext.Session.SetString("TidalChallenge", challengeResponse.Data);

            ApiResponse<string> authLinkResponse = _authService.BuildTidalAuthLink(stateResponse.Data, challengeResponse.Data);
            if (!authLinkResponse.Success) return BadRequest(authLinkResponse.Message);

            return Redirect(authLinkResponse.Data);


        }
        
        [HttpGet("tidal/callback")]
        public async Task<IActionResult> TidalCallback([FromQuery] string code, [FromQuery] string state)
        {
            var sessionState = HttpContext.Session.GetString("TidalAuthState");
            if (sessionState == null || sessionState != state)
            {
                return BadRequest("Invalid State");
            }
            var verifier = HttpContext.Session.GetString("TidalVerifier");
            if (verifier == null)
            {
                return BadRequest("Invalid verifier");
            }
            ApiResponse<ResponseToken> tokensResponse = await _authService.TidalTokenResponse(code,verifier);
            HttpContext.Session.SetString("TidalTokens", JsonSerializer.Serialize(tokensResponse.Data));

            return Redirect($"{_frontendUrl}login/success?state={state}");
        }
        [HttpGet("tidal/tokens")]
        public async Task<IActionResult> TidalGetTokens([FromQuery] string state)
        {
            var sessionState = HttpContext.Session.GetString("TidalAuthState");
            if (sessionState == null || state != sessionState)
            {
                return BadRequest("Invalid State");
            }
            var tokens = HttpContext.Session.GetString("TidalTokens");
            if (tokens == null) return BadRequest("No tokens found");

            return Ok(tokens);
        }
    }
    
}