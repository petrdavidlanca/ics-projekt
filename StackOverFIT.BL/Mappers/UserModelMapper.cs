using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;

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
        => entity is null
            ? UserDetailModel.Empty
            : new UserDetailModel
            {
                Id = entity.Id,
                Username = entity.Username,
                Email = entity.Email,
                RealName = entity.RealName,
                Surname = entity.Surname,
                TotalNumOfGames = entity.TotalNumOfGames,
                TotalNumOfLibraries = entity.TotalNumOfLibraries
            };

    public override UserEntity MapToEntity(UserDetailModel model)
        => new()
        {
            Id = model.Id,
            Username = model.Username,
            Email = model.Email,
            RealName = model.RealName,
            Surname = model.Surname,
            TotalNumOfGames = model.TotalNumOfGames,
            TotalNumOfLibraries = model.TotalNumOfLibraries
        };
}