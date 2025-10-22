namespace RONBlitz.Server.Models
{
    public class PlayerScore
    {
        public int Id { get; set; }
        public string Player { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}
