using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StackOverFIT.DAL.Factories;

/// <summary>
///     EF Core CLI migration generation uses this DbContext to create model and migration
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<StackOverFITDbContext>
{
    private readonly DbContextSqLiteFactory _dbContextSqLiteFactory = new("StackOverFIT.db");

    public StackOverFITDbContext CreateDbContext(string[] args) => _dbContextSqLiteFactory.CreateDbContext();
}
