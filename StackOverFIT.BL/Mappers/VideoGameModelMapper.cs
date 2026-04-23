using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
using System.Collections.ObjectModel;

namespace StackOverFIT.BL.Mappers;

public class VideoGameModelMapper : ModelMapperBase<VideoGameEntity, VideoGameListModel, VideoGameDetailModel>
{
    public override VideoGameListModel MapToListModel(VideoGameEntity? entity)
        => entity is null
            ? VideoGameListModel.Empty
            : new VideoGameListModel
            {
                Id = entity.Id,
                Name = entity.Name,
                ImageUrl = entity.ImageUrl
            };

    public override VideoGameDetailModel MapToDetailModel(VideoGameEntity? entity)
        => entity is null
            ? VideoGameDetailModel.Empty
            : new VideoGameDetailModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                PegiRating = entity.PegiRating,
                ImageUrl = entity.ImageUrl,
                LibraryId = entity.LibraryId,
                GameGenres = new ObservableCollection<DAL.Enums.GameGenres>(entity.GameGenres)
            };

    public override VideoGameEntity MapToEntity(VideoGameDetailModel model)
        => new()
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            PegiRating = model.PegiRating,
            ImageUrl = model.ImageUrl,
            LibraryId = model.LibraryId,
            GameGenres = model.GameGenres.ToList()
        };
}