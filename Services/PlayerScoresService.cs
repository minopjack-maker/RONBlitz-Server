using RONBlitz.Server.Models;

namespace RONBlitz.Server.Services
{
    public class PlayerScoresService
    {
        private static readonly List<PlayerScore> _scores = new()
        {
            new PlayerScore { Player = "Jorden", Score = 2340 },
            new PlayerScore { Player = "Omen", Score = 2100 },
            new PlayerScore { Player = "Robin", Score = 1990 },
            new PlayerScore { Player = "Noah", Score = 1785 },
            new PlayerScore { Player = "Max", Score = 1660 }
        };

        public List<PlayerScore> GetAllScores() =>
            _scores.OrderByDescending(s => s.Score).ToList();

        public void AddOrUpdateScore(string player, int score)
        {
            var existing = _scores.FirstOrDefault(
                s => s.Player.Equals(player, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                existing.Score = score;
            }
            else
            {
                _scores.Add(new PlayerScore { Player = player, Score = score });
            }
        }

        public void DeleteScore(string player)
        {
            var existing = _scores.FirstOrDefault(
                s => s.Player.Equals(player, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                _scores.Remove(existing);
            }
        }
    }
}
