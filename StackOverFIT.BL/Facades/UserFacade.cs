using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Mappers;
using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
using StackOverFIT.DAL.Mappers;
using StackOverFIT.DAL.Repositories;
using StackOverFIT.DAL.UnitOfWork;

namespace StackOverFIT.BL.Facades;

public class UserFacade(
    IUnitOfWorkFactory unitOfWorkFactory,
    UserModelMapper modelMapper)
    : FacadeBase<user_entity, UserListModel, UserDetailModel, UserEntityMapper>(
        unitOfWorkFactory, modelMapper), IUserFacade
{

}