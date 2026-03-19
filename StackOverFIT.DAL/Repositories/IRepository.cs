using StackOverFIT.DAL.Entities;

namespace StackOverFIT.DAL.Repositories;

public interface IRepository<TEntity> where TEntity : class, IEntity
{
    IQueryable<TEntity> Get();
    
    Task<TEntity?> GetByIdAsync(Guid id);
    
    TEntity Insert(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task DeleteAsync(Guid id);

    ValueTask<bool> ExistsAsync(TEntity entity);
}