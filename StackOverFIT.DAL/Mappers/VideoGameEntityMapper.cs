using StackOverFIT.DAL.Entities;

namespace StackOverFIT.DAL.Mappers;

public class VideoGameEntityMapper : IEntityMapper<VideoGameEntity>
{
    public void MapToExistingEntity(VideoGameEntity existingEntity, VideoGameEntity newEntity)
    {
        existingEntity.Name = newEntity.Name;
        existingEntity.Description = newEntity.Description;
        existingEntity.PegiRating = newEntity.PegiRating;
        existingEntity.ImageUrl = newEntity.ImageUrl;
        existingEntity.LibraryId = newEntity.LibraryId;
    }
}