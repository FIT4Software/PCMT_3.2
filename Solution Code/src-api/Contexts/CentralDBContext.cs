using Microsoft.EntityFrameworkCore;

namespace Contexts
{
    public class CentralDBContext : DbContext
    {
        public CentralDBContext(DbContextOptions<CentralDBContext> options) : base(options) {}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                .EnableSensitiveDataLogging() 
                .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, Microsoft.Extensions.Logging.LogLevel.Information);
        }
    }
}