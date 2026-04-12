using StackOverFIT.DAL.Entities;

namespace StackOverFIT.DAL.Repositories;

public interface IRepository<TEntity> where TEntity : class, IEntity
{
    IQueryable<TEntity> Get();

    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    TEntity Insert(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    ValueTask<bool> ExistsAsync(TEntity entity, CancellationToken cancellationToken = default);
}