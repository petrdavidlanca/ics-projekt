using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;

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
                NumOfGames = entity.NumOfGames
            };

    public override LibraryDetailModel MapToDetailModel(LibraryEntity? entity)
        => entity is null
            ? LibraryDetailModel.Empty
            : new LibraryDetailModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                NumOfGames = entity.NumOfGames
            };

    public override LibraryEntity MapToEntity(LibraryDetailModel model)
        => new()
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            NumOfGames = model.NumOfGames
        };
}