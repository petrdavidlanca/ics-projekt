using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;

namespace StackOverFIT.BL.Mappers;

public class VideoGameModelMapper : ModelMapperBase<video_game_entity, VideoGameListModel, VideoGameDetailModel>
{
    public override VideoGameListModel MapToListModel(video_game_entity? entity)
        => entity is null
            ? VideoGameListModel.Empty
            : new VideoGameListModel
            {
                Id = entity.Id,
                name = entity.name,
                ImageUrl = entity.ImageUrl
            };

    public override VideoGameDetailModel MapToDetailModel(video_game_entity? entity)
        => entity is null
            ? VideoGameDetailModel.Empty
            : new VideoGameDetailModel
            {
                Id = entity.Id,
                name = entity.name,
                description = entity.description,
                pegi_rating = entity.pegi_rating,
                ImageUrl = entity.ImageUrl
            };

    public override video_game_entity MapToEntity(VideoGameDetailModel model)
        => new()
        {
            Id = model.Id,
            name = model.name,
            description = model.description,
            pegi_rating = model.pegi_rating,
            ImageUrl = model.ImageUrl
        };
}