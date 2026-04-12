using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StackOverFIT.DAL.Factories;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<StackOverFITDbContext>
{
    private readonly DbContextSqLiteFactory _DbContextSqLiteFactory = new("StackOverFIT.db");

    public StackOverFITDbContext CreateDbContext(string[] args) => _DbContextSqLiteFactory.CreateDbContext();
}
