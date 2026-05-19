using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Mappers;
using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
using StackOverFIT.DAL.Mappers;
using StackOverFIT.DAL.Repositories;
using StackOverFIT.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace StackOverFIT.BL.Facades;

public class UserFacade(
    IUnitOfWorkFactory unitOfWorkFactory,
    UserModelMapper modelMapper)
    : FacadeBase<UserEntity, UserListModel, UserDetailModel, UserEntityMapper>(
        unitOfWorkFactory, modelMapper), IUserFacade
{
    public override async Task<UserDetailModel?> GetAsync(Guid id)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        var entity = await uow.GetRepository<UserEntity, UserEntityMapper>()
            .Get()
            .Include(u => u.GameLibrary)
                .ThenInclude(l => l.VideoGames)
            .FirstOrDefaultAsync(u => u.Id == id)
            .ConfigureAwait(false);
        return entity is null
            ? null
            : ModelMapper.MapToDetailModel(entity);
    }
    public async Task<IEnumerable<UserListModel>> GetFilteredUsersAsync(string? searchTerm, string? sortBy, bool ascending = true)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        IQueryable<UserEntity> query = uow.GetRepository<UserEntity, UserEntityMapper>().Get();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(u => u.Username.ToLower().Contains(searchTerm) ||
                                     (u.Email != null && u.Email.ToLower().Contains(searchTerm)));
        }

        query = sortBy?.ToLower() switch
        {
            "username" => ascending ? query.OrderBy(u => u.Username) : query.OrderByDescending(u => u.Username),
            "email" => ascending ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
            _ => query.OrderBy(u => u.Username)
        };

        var entities = await query.ToListAsync().ConfigureAwait(false);
        return ModelMapper.MapToListModel(entities);
    }
}