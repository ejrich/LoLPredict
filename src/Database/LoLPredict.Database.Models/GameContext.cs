using Microsoft.EntityFrameworkCore;

namespace LoLPredict.Database.Models
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patch>()
                .HasKey(entity => new { entity.Major, entity.Minor, entity.Version });
        }

        public DbSet<Champion> Champions { get; set; }
        public DbSet<GameResult> Results { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Patch> Patches { get; set; }
        public DbSet<Summoner> Summoners { get; set; }
    }
}
