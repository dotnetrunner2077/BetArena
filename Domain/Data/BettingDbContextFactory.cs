using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Domain.Data;

public class BettingDbContextFactory : IDesignTimeDbContextFactory<BettingDbContext>
{
    public BettingDbContext CreateDbContext(string[] args)
    {
        const string connectionString =
            "server=localhost;port=3306;database=BettingDb;user=root;password=123456";

        var optionsBuilder = new DbContextOptionsBuilder<BettingDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new BettingDbContext(optionsBuilder.Options);
    }
}
