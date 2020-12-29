using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Boardgames.WebServer.Data
{
    public class SqlServerApplicationDbContext : ApplicationDbContext
    {
        public SqlServerApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Data Source=(LocalDb)\\MSSQLLocalDB;Database=Boardgames;Trusted_Connection=True;MultipleActiveResultSets=true;Integrated Security=SSPI;";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}