using Microsoft.EntityFrameworkCore;

namespace Contexts
{
    public class SiteDBContext : DbContext
    {
        public SiteDBContext(DbContextOptions<SiteDBContext> options, IConfiguration config) : base(options) {
            _configuration = config;
        }
        private readonly IConfiguration _configuration;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public void setOnPremConnection(string server)
        {
            var connString = _configuration.GetConnectionString("SiteDB");
            connString = connString.Replace("{server}", server);
            this.Database.SetConnectionString(connString);
        }        
    }
}