using Microsoft.EntityFrameworkCore;
using RONBlitz.Server.Models;

namespace RONBlitz.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PlayerScore> PlayerScores { get; set; } = null!;
    }
}
