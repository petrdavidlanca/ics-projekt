using StackOverFIT.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using StackOverFIT.DAL.Mappers;

namespace StackOverFIT.DAL.Repositories;

public class Repository<TEntity>(DbContext dbContext, IEntityMapper<TEntity> entityMapper)
    : IRepository<TEntity> where TEntity : class, IEntity
{
    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();
    
    public IQueryable<TEntity> Get() => _dbSet;
    
    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
    }

    public TEntity Insert(TEntity entity) => _dbSet.Add(entity).Entity;

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        TEntity? existEntity = await GetByIdAsync(entity.Id);

        if (existEntity != null)
        {
            entityMapper.MapToExistingEntity(existEntity, entity);
        }

        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        TEntity? entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }
    
    public async ValueTask<bool> ExistsAsync(TEntity entity)
    {
        return entity.Id != Guid.Empty && await _dbSet.AnyAsync(e => e.Id == entity.Id);
    }
    
}