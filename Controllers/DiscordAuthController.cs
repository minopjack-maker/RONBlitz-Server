using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace RONBlitz.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscordAuthController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public DiscordAuthController(IConfiguration config)
        {
            _httpClient = new HttpClient();
            _config = config;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var clientId = _config["Discord:ClientId"];
            var redirectUri = _config["Discord:RedirectUri"];

            var discordAuthUrl = $"https://discord.com/oauth2/authorize" +
                                 $"?client_id={clientId}" +
                                 $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                                 $"&response_type=code" +
                                 $"&scope=identify";

            return Redirect(discordAuthUrl);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            var clientId = _config["Discord:ClientId"];
            var clientSecret = _config["Discord:ClientSecret"];
            var redirectUri = _config["Discord:RedirectUri"];

            var tokenResponse = await _httpClient.PostAsync("https://discord.com/api/oauth2/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = clientId!,
                    ["client_secret"] = clientSecret!,
                    ["grant_type"] = "authorization_code",
                    ["code"] = code,
                    ["redirect_uri"] = redirectUri!
                }));

            var tokenData = JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync()).RootElement;
            if (!tokenData.TryGetProperty("access_token", out var accessTokenElement))
                return BadRequest("Failed to obtain Discord access token.");

            var accessToken = accessTokenElement.GetString();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var userResponse = await _httpClient.GetStringAsync("https://discord.com/api/users/@me");

            // take the JSON from Discord and encode it safely for the URL
            var encodedUser = Uri.EscapeDataString(
                Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(userResponse))
            );

            // point this to your actual frontend site
            var frontendUrl = $"https://ronblitz.netlify.app/auth/callback?user={encodedUser}";

            return Redirect(frontendUrl);

        }

        // ✅ Add this endpoint
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            return Ok(new { status = "ok" });
        }
    }
}
