using StackOverFIT.DAL.Entities;

namespace StackOverFIT.DAL.Mappers;

public class VideoGameEntityMapper : IEntityMapper<video_game_entity>
{
    public void MapToExistingEntity(video_game_entity existingEntity, video_game_entity newEntity)
    {
        existingEntity.name = newEntity.name;
        existingEntity.description = newEntity.description;
        existingEntity.pegi_rating = newEntity.pegi_rating;
        existingEntity.ImageUrl = newEntity.ImageUrl;
    }
}