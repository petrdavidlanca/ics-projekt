using StackOverFIT.DAL.Entities;
using StackOverFIT.DAL.Mappers;
using StackOverFIT.DAL.Repositories;

namespace StackOverFIT.DAL.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<TEntity> GetRepository<TEntity, TEntityMapper>()
        where TEntity : class, IEntity
        where TEntityMapper : class, IEntityMapper<TEntity>;

    Task CommitAsync(CancellationToken cancellationToken = default);
}