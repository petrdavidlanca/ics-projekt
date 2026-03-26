using StackOverFIT.DAL.Entities;

namespace StackOverFIT.DAL.Mappers;

public class LibraryEntityMapper : IEntityMapper<LibraryEntity>
{
    public void MapToExistingEntity(LibraryEntity existingEntity, LibraryEntity newEntity)
    {
        existingEntity.Name = newEntity.Name;
        existingEntity.Description = newEntity.Description;
        existingEntity.NumOfGames = newEntity.NumOfGames;
    }
}