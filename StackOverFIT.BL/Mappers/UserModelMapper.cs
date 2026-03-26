using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;

namespace StackOverFIT.BL.Mappers;

public class UserModelMapper : ModelMapperBase<user_entity, UserListModel, UserDetailModel>
{
    public override UserListModel MapToListModel(user_entity? entity)
        => entity is null
            ? UserListModel.empty
            : new UserListModel
            {
                Id = entity.Id,
                username = entity.username,
                email = entity.email
            };

    public override UserDetailModel MapToDetailModel(user_entity? entity)
        => entity is null
            ? UserDetailModel.Empty
            : new UserDetailModel
            {
                Id = entity.Id,
                username = entity.username,
                email = entity.email,
                real_name = entity.real_name,
                surname = entity.surname,
                total_num_of_games = entity.total_num_of_games,
                total_num_of_libraries = entity.total_num_of_libraries
            };

    public override user_entity MapToEntity(UserDetailModel model)
        => new()
        {
            Id = model.Id,
            username = model.username,
            email = model.email,
            real_name = model.real_name,
            surname = model.surname,
            total_num_of_games = model.total_num_of_games,
            total_num_of_libraries = model.total_num_of_libraries
        };
}