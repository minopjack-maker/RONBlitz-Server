using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace RONBlitz.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScoresController : ControllerBase
    {
        // ✅ Temporary in-memory scores (can be replaced later with DB)
        private static readonly List<ScoreEntry> Scores = new()
        {
            new() { Player = "Omen", Score = 1200 },
            new() { Player = "Robin", Score = 950 },
            new() { Player = "Noah", Score = 880 }
        };

        // 🌍 Public - Anyone can view
        [HttpGet]
        [AllowAnonymous] // ✅ Public access for viewing scores
        public IActionResult GetScores()
        {
            return Ok(Scores.OrderByDescending(s => s.Score));
        }

        // 🔒 Admin-only - requires JWT token
        [Authorize]
        [HttpPost]
        public IActionResult AddScore([FromBody] ScoreEntry entry)
        {
            if (string.IsNullOrWhiteSpace(entry.Player))
                return BadRequest("Player name required.");

            var existing = Scores.FirstOrDefault(s => s.Player == entry.Player);
            if (existing != null)
                existing.Score = entry.Score;
            else
                Scores.Add(entry);

            return Ok(new { message = "Score added/updated ✅", Scores });
        }

        // 🔒 Admin-only - remove player score
        [Authorize]
        [HttpDelete("{player}")]
        public IActionResult DeleteScore(string player)
        {
            var existing = Scores.FirstOrDefault(s => 
                s.Player.Equals(player, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
                return NotFound($"No score found for {player}.");

            Scores.Remove(existing);
            return Ok(new { message = $"Removed {player} from leaderboard ❌", Scores });
        }
    }

    public class ScoreEntry
    {
        public string Player { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}
