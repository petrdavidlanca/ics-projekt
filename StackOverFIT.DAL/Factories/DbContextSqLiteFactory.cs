using Microsoft.EntityFrameworkCore;
using StackOverFIT.DAL;

namespace StackOverFIT.DAL.Factories;

public class DbContextSqLiteFactory : IDbContextFactory<StackOverFITDbContext>
{
    private readonly DbContextOptionsBuilder<StackOverFITDbContext> _contextOptionsBuilder = new();

    public DbContextSqLiteFactory(string databaseName)
    {
        _contextOptionsBuilder.UseSqlite($"Data Source={databaseName};Cache=Shared");

    }

    public StackOverFITDbContext CreateDbContext() => new(_contextOptionsBuilder.Options);
}
