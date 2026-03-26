using StackOverFIT.DAL.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace StackOverFIT.DAL.Migrator;

public class DbMigrator(IDbContextFactory<StackOverFITDbContext> DbContextFactory, IOptions<DALOptions> options)
    : IDbMigrator
{
    public void Migrate()
    {
        using StackOverFITDbContext DbContext = DbContextFactory.CreateDbContext();

        if(options.Value.RecreateDatabaseEachTime)
        {
            DbContext.Database.EnsureDeleted();
        }

        DbContext.Database.EnsureCreated();
    }
}
