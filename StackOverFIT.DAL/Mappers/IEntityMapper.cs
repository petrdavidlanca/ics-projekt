using StackOverFIT.DAL.Entities;

namespace StackOverFIT.DAL.Mappers;

public interface IEntityMapper<in TEntity> where TEntity : class, IEntity
{
    void MapToExistingEntity(TEntity existingEntity, TEntity newEntity);
}