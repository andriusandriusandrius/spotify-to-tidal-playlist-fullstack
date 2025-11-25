using System.Text.Json;
using backend.Configurations;
using backend.DTOs;
using backend.Exceptions;
using backend.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
namespace backend.Api
{
    public class TidalStateData
    {
        public string Verifier { get; set; } = null!;
    }

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly string _frontendUrl;
        private readonly IDatabase _redis;
        public AuthController(IAuthService authService, IConnectionMultiplexer redis, IOptions<TidalAuthOptions> config)
        {
            _authService = authService;
            _frontendUrl = config.Value.FrontendRedir ?? throw  new InvalidOperationException("FrontendRedir not defined");
            _redis = redis.GetDatabase();
        }
        
        [HttpGet("spotify/login")]
        public async Task<IActionResult> SpotifyLogin()
        {

            string generateStateResponse = _authService.GenerateState();

            await _redis.StringSetAsync($"spotify:state:{generateStateResponse}", "pending", TimeSpan.FromMinutes(5));

            string authLinkResponse = _authService.BuildSpotifyAuthLink(generateStateResponse);

            return Redirect(authLinkResponse);
        }

        [HttpGet("spotify/callback")]
        public async Task<IActionResult> SpotifyCallback([FromQuery] string code, [FromQuery] string state)
        {
            var sessionState = await _redis.StringGetAsync($"spotify:state:{state}");
            if (!sessionState.HasValue)
            {
                throw new StateException ("Invalid Spotify state");
            }
            await _redis.KeyDeleteAsync($"spotify:state:{state}");
            ResponseTokenDTO tokensResponse = await _authService.SpotifyTokenResponse(code);

            await _redis.StringSetAsync($"spotify:tokens:{state}", JsonSerializer.Serialize(tokensResponse), TimeSpan.FromHours(1)); 

            return Redirect($"{_frontendUrl}to/success?state={state}");
        }
        [HttpGet("spotify/tokens")]
        public async Task<IActionResult> SpotifyGetTokens([FromQuery] string state)
        {

            var tokens = await _redis.StringGetAsync($"spotify:tokens:{state}");
            if (!tokens.HasValue) return NotFound("No tokens found");

             var tokensString = tokens.ToString(); 

             var tokensObj = JsonSerializer.Deserialize<ResponseTokenDTO>(tokensString);

            return Ok(tokensObj);
        }

        [HttpGet("tidal/login")]
        public async Task<IActionResult> TidalLogin()
        {
            string state = _authService.GenerateState();
            string verifier = _authService.GenerateVerifier();
            string challenge = _authService.GenerateCodeChallenge(verifier);

            var data = JsonSerializer.Serialize(new TidalStateData { Verifier=verifier});
            await _redis.StringSetAsync($"tidal:state:{state}", data, TimeSpan.FromMinutes(5));

            string authLink = _authService.BuildTidalAuthLink(state, challenge);
            return Redirect(authLink);


        }
        
        [HttpGet("tidal/callback")]
        public async Task<IActionResult> TidalCallback([FromQuery] string code, [FromQuery] string state)
        {
            var storedData = await _redis.StringGetAsync($"tidal:state:{state}");

            if (!storedData.HasValue)
            {
                throw new StateException("Invalid Tidal state");
            }

            var storedObj = JsonSerializer.Deserialize<TidalStateData>(storedData!);

            string verifier = storedObj!.Verifier;
            Console.WriteLine(verifier);
            ResponseTokenDTO tokens = await _authService.TidalTokenResponse(code,verifier);
            
            await _redis.StringSetAsync($"tidal:tokens:{state}", JsonSerializer.Serialize(tokens), TimeSpan.FromHours(1));

            return Redirect($"{_frontendUrl}transfer/success?state={state}");
        }
        [HttpGet("tidal/tokens")]
        public async Task<IActionResult> TidalGetTokens([FromQuery] string state)
        {
            var tokens = await _redis.StringGetAsync($"tidal:tokens:{state}");
            if (!tokens.HasValue) return NotFound("No tokens found");

            var tokensString = tokens.ToString(); 

            var tokensObj = JsonSerializer.Deserialize<ResponseTokenDTO>(tokensString);

            return Ok(tokensObj);
        }
    }
    
}