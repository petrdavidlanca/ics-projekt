using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackOverFIT.DAL.Entities;
using StackOverFIT.DAL.Mappers;
using StackOverFIT.DAL.Repositories;

namespace StackOverFIT.DAL.UnitOfWork;

public sealed class UnitOfWork(DbContext dbContext, IServiceScope serviceScope) : IUnitOfWork
{
    private readonly DbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly IServiceScope _serviceScope = serviceScope ?? throw new ArgumentNullException(nameof(serviceScope));

    public IRepository<TEntity> GetRepository<TEntity, TEntityMapper>()
        where TEntity : class, IEntity
        where TEntityMapper : class, IEntityMapper<TEntity>
        => new Repository<TEntity>(_dbContext, _serviceScope.ServiceProvider.GetRequiredService<TEntityMapper>());

    public async Task CommitAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync().ConfigureAwait(false);
        _serviceScope.Dispose();
    }
}