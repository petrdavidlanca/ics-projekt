using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
using StackOverFIT.BL.Facades;

namespace StackOverFIT.BL.Facades;

public interface IUserFacade : IFacade<user_entity, UserListModel, UserDetailModel>
{
}
