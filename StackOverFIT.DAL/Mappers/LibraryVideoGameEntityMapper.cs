using StackOverFIT.DAL.Entities;

namespace StackOverFIT.DAL.Mappers;

public class LibraryVideoGameEntityMapper : IEntityMapper<LibraryVideoGameEntity>
{
    public void MapToExistingEntity(LibraryVideoGameEntity existingEntity, LibraryVideoGameEntity newEntity)
    {
        existingEntity.LibraryId = newEntity.LibraryId;
        existingEntity.VideoGameId = newEntity.VideoGameId;
    }
}