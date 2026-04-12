using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace StackOverFIT.DAL.UnitOfWork;

public class UnitOfWorkFactory(
    IDbContextFactory<StackOverFITDbContext> dbContextFactory,
    IServiceScopeFactory serviceScopeFactory) : IUnitOfWorkFactory
{
    public IUnitOfWork Create()
    {
        var scope = serviceScopeFactory.CreateScope();
        return new UnitOfWork(dbContextFactory.CreateDbContext(), scope);
    }
}