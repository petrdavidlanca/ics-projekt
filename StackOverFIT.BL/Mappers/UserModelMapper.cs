using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
using System.Collections.ObjectModel;

namespace StackOverFIT.BL.Mappers;

public class UserModelMapper : ModelMapperBase<UserEntity, UserListModel, UserDetailModel>
{
    public override UserListModel MapToListModel(UserEntity? entity)
        => entity is null
            ? UserListModel.Empty
            : new UserListModel
            {
                Id = entity.Id,
                Username = entity.Username,
                Email = entity.Email
            };

    public override UserDetailModel MapToDetailModel(UserEntity? entity)
    {
        if (entity is null) 
            return UserDetailModel.Empty;

        var libraryMapper = new LibraryModelMapper();

        return new UserDetailModel
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email,
            RealName = entity.RealName,
            Surname = entity.Surname,
            TotalNumOfGames = entity.GameLibrary?.Sum(lib => lib.VideoGames?.Count ?? 0) ?? 0,
            TotalNumOfLibraries = entity.GameLibrary?.Count ?? 0,
            GameLibrary = new ObservableCollection<LibraryListModel>(libraryMapper.MapToListModel(entity.GameLibrary))
        };
    }

    public override UserEntity MapToEntity(UserDetailModel model)
        => new()
        {
            Id = model.Id,
            Username = model.Username,
            Email = model.Email,
            RealName = model.RealName,
            Surname = model.Surname,
            TotalNumOfGames = model.TotalNumOfGames,
            TotalNumOfLibraries = model.TotalNumOfLibraries,
            GameLibrary = model.GameLibrary.Select(l => new LibraryEntity { Id = l.Id, Name = l.Name }).ToList()
        };
}