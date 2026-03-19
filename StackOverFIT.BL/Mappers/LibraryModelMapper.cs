using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;

namespace StackOverFIT.BL.Mappers;

public class LibraryModelMapper : ModelMapperBase<library_entity, LibraryListModel, LibraryDetailModel>
{
    public override LibraryListModel MapToListModel(library_entity? entity)
        => entity is null
            ? LibraryListModel.Empty
            : new LibraryListModel
            {
                Id = entity.Id,
                name = entity.name,
                num_of_games = entity.num_of_games
            };

    public override LibraryDetailModel MapToDetailModel(library_entity? entity)
        => entity is null
            ? LibraryDetailModel.Empty
            : new LibraryDetailModel
            {
                Id = entity.Id,
                name = entity.name,
                description = entity.description,
                num_of_games = entity.num_of_games
            };

    public override library_entity MapToEntity(LibraryDetailModel model)
        => new()
        {
            Id = model.Id,
            name = model.name,
            description = model.description,
            num_of_games = model.num_of_games
        };
}