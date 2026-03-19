using StackOverFIT.DAL.Entities;

namespace StackOverFIT.DAL.Mappers;

public class UserEntityMapper : IEntityMapper<user_entity>
{
    public void MapToExistingEntity(user_entity existingEntity, user_entity newEntity)
    {
        existingEntity.username = newEntity.username;
        existingEntity.email = newEntity.email;
        existingEntity.real_name = newEntity.real_name;
        existingEntity.surname = newEntity.surname;
        existingEntity.total_num_of_games = newEntity.total_num_of_games;
        existingEntity.total_num_of_libraries = newEntity.total_num_of_libraries;
    }
}