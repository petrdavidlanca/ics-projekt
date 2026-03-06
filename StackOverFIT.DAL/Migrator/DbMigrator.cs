using StackOverFIT.DAL.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace StackOverFIT.DAL.Migrator;

public class DbMigrator(IDbContextFactory<StackOverFITDbContext> dbContextFactory, IOptions<DALOptions> options)
    : IDbMigrator
{
    public void Migrate()
    {
        using StackOverFITDbContext dbContext = dbContextFactory.CreateDbContext();

        if(options.Value.RecreateDatabaseEachTime)
        {
            dbContext.Database.EnsureDeleted();
        }

        // Ensures that database is created applying the latest state
        // Application of migration later on may fail
        // If you want to use migrations, you should create database by calling  dbContext.Database.Migrate() instead
        dbContext.Database.EnsureCreated();
    }
}
