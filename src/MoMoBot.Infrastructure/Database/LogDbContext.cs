using Microsoft.EntityFrameworkCore;

namespace MoMoBot.Infrastructure.Database
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
        {

        }
    }
}
