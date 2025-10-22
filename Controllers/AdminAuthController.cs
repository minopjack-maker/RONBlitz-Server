using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RONBlitz.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminAuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AdminAuthController(IConfiguration config)
        {
            _config = config;
        }

        // ✅ Store pre-hashed passwords securely
        private static readonly Dictionary<string, string> Users = new()
        {
            { "omen", "$2a$11$uhs6QOj1NUSNLqewI08yu.UEeCieV8ZBMd375hdLv70pVxUZlHcoW" },
            { "robin", "$2a$11$.05FUJQaBfc0cuUrvJqU1uiaiU73c8jg9xYiPFA7G6TQesWCstjvS" },
            { "noah", "$2a$11$9oxwt./lsSWnreMIC51aQ.rdD7woEGx8T4ukWT0q5rbkE219mZm9a" },
            { "jorden", "$2a$11$qTXB7uWVs5XHd1zmduPlF.gnPZUQ41hTHcO/R2VFYz3d5sToAXK9m" }
        };

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest("Username and password required.");

                if (Users.TryGetValue(request.Username, out var hashedPassword) &&
                    BCrypt.Net.BCrypt.Verify(request.Password, hashedPassword))
                {
                    var token = GenerateJwtToken(request.Username);
                    return Ok(new { token, username = request.Username, message = "Login successful ✅" });
                }

                return Unauthorized("Invalid username or password.");
            }
            catch (Exception ex)
            {
                // ✅ Safer logging method for Render
                System.Diagnostics.Debug.WriteLine($"[ERROR] {ex}");
                return StatusCode(500, "Internal server error — see logs for details.");
            }
        }

        // ✅ New test endpoint to verify the controller works
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("✅ AdminAuthController is reachable");
        }

        private string GenerateJwtToken(string username)
        {
            var key = _config["Jwt:Key"] ?? "X3!vPz81LmAq47$Rf9KdB2@tYnHjQ5uC"; // 🔒 256-bit key (secure)
            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
