using StackOverFIT.DAL.Entities;

namespace StackOverFIT.DAL.Mappers;

public class LibraryEntityMapper : IEntityMapper<library_entity>
{
    public void MapToExistingEntity(library_entity existingEntity, library_entity newEntity)
    {
        existingEntity.name = newEntity.name;
        existingEntity.description = newEntity.description;
        existingEntity.num_of_games = newEntity.num_of_games;
    }
}