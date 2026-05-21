using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;

namespace StackOverFIT.BL.Facades;

public interface IUserFacade : IFacade<UserEntity, UserListModel, UserDetailModel>
{
    Task<IEnumerable<UserListModel>> GetFilteredUsersAsync(string? searchTerm, string? sortBy, bool ascending = true);
}