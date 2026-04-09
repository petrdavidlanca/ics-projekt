using StackOverFIT.DAL.Entities;

namespace StackOverFIT.DAL.Mappers;

public class UserEntityMapper : IEntityMapper<UserEntity>
{
    public void MapToExistingEntity(UserEntity existingEntity, UserEntity newEntity)
    {
        existingEntity.Username = newEntity.Username;
        existingEntity.Email = newEntity.Email;
        existingEntity.RealName = newEntity.RealName;
        existingEntity.Surname = newEntity.Surname;
        existingEntity.TotalNumOfGames = newEntity.TotalNumOfGames;
        existingEntity.TotalNumOfLibraries = newEntity.TotalNumOfLibraries;
    }
}