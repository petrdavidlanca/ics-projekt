using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
using System.Collections.ObjectModel;

namespace StackOverFIT.BL.Mappers;

public class LibraryModelMapper : ModelMapperBase<LibraryEntity, LibraryListModel, LibraryDetailModel>
{
    public override LibraryListModel MapToListModel(LibraryEntity? entity)
        => entity is null
            ? LibraryListModel.Empty
            : new LibraryListModel
            {
                Id = entity.Id,
                Name = entity.Name,
                NumOfGames = entity.VideoGames?.Count ?? 0
            };

    public override LibraryDetailModel MapToDetailModel(LibraryEntity? entity)
    {
        if (entity is null) 
            return LibraryDetailModel.Empty;

        var videoGameMapper = new VideoGameModelMapper();

        return new LibraryDetailModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            NumOfGames = entity.VideoGames?.Count ?? 0,
            UserId = entity.UserEntityId ?? Guid.Empty,
            VideoGames = new ObservableCollection<VideoGameListModel>(videoGameMapper.MapToListModel(entity.VideoGames))
        };
    }

    public override LibraryEntity MapToEntity(LibraryDetailModel model)
        => new()
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            NumOfGames = model.NumOfGames,
            UserEntityId = model.UserId == Guid.Empty ? null : model.UserId,
            VideoGames = model.VideoGames.Select(v => new VideoGameEntity { Id = v.Id, Name = v.Name, Description = "", PegiRating = 0 }).ToList()
        };
}