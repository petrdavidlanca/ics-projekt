using Microsoft.EntityFrameworkCore;
using StackOverFIT.DAL;

namespace StackOverFIT.DAL.Factories;

public class DbContextSqLiteFactory : IDbContextFactory<StackOverFITDbContext>
{
    private readonly DbContextOptionsBuilder<StackOverFITDbContext> _contextOptionsBuilder = new();

    public DbContextSqLiteFactory(string databaseName)
    {
        ////May be helpful for ad-hoc testing, not drop in replacement, needs some more configuration.
        //builder.UseSqlite($"Data Source =:memory:;");
        _contextOptionsBuilder.UseSqlite($"Data Source={databaseName};Cache=Shared");

        ////Enable in case you want to see tests details, enabled may cause some inconsistencies in tests
        //_contextOptionsBuilder.EnableSensitiveDataLogging();
        //_contextOptionsBuilder.LogTo(Console.WriteLine);
    }

    public StackOverFITDbContext CreateDbContext() => new(_contextOptionsBuilder.Options);
}
