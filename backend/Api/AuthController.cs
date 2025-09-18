using backend.DTOs;
using backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace backend.Api {
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
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
            ApiResponse<SpotifyResponseToken> tokensResponse = await _authService.SpotifyTokenResponse(code, state);
            if (!tokensResponse.Success) return BadRequest(tokensResponse.Message);
            return Ok(tokensResponse);       
        }
    }
}