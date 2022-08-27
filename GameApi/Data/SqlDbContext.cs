using GameApi.Model;
using Microsoft.EntityFrameworkCore;

namespace GameApi.Infrastructure
{

    public class SqlDbContext : DbContext
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options)
            : base(options)
        {
        }

        public DbSet<GameInstance>? GameInstance { get; set; }

        public DbSet<GameStep>? GameStep { get; set; }

        public DbSet<GameUser>? GameUser { get; set; }

        public DbSet<Leaderboard>? Leaderboard { get; set; }
    }
}