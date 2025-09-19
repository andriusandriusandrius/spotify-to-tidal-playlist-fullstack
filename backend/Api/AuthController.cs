using System.Text.Json;
using backend.DTOs;
using backend.Service;
using DotNetEnv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace backend.Api {
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
        [HttpGet("login")]
        public IActionResult Login()
        {

            ApiResponse<string> generateStateResponse = _authService.GenerateState();
            if (!generateStateResponse.Success) return BadRequest(generateStateResponse.Message);

            HttpContext.Session.SetString("AuthState", generateStateResponse.Data);

            ApiResponse<string> authLinkResponse = _authService.BuildSpotifyAuthLink(generateStateResponse.Data);
            if (!authLinkResponse.Success) return BadRequest(authLinkResponse.Message);

            return Redirect(authLinkResponse.Data);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            var sessionState = HttpContext.Session.GetString("AuthState");
            if (sessionState == null || sessionState != state)
            {
                return BadRequest("Invalid State");
            }
            ApiResponse<SpotifyResponseToken> tokensResponse = await _authService.SpotifyTokenResponse(code);
            HttpContext.Session.SetString("SpotifyTokens", JsonSerializer.Serialize(tokensResponse.Data));

            return Redirect($"{_frontendUrl}login/success?state={state}");
        }
        [HttpGet("tokens")]
        public async Task<IActionResult> GetTokens([FromQuery] string state)
        {
            var sessionState = HttpContext.Session.GetString("AuthState");
            if (sessionState == null || state != sessionState)
            {
                return BadRequest("Invalid State");
            }
            var tokens = HttpContext.Session.GetString("SpotifyTokens");
            if (tokens == null) return BadRequest("No tokens found");

            return Ok(tokens);
        }
    }
}