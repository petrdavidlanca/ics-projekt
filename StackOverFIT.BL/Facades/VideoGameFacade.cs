using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Mappers;
using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
using StackOverFIT.DAL.Mappers;
using StackOverFIT.DAL.Repositories;
using StackOverFIT.DAL.UnitOfWork;

namespace StackOverFIT.BL.Facades;

public class VideoGameFacade(
    IUnitOfWorkFactory unitOfWorkFactory,
    VideoGameModelMapper modelMapper)
    : FacadeBase<video_game_entity, VideoGameListModel, VideoGameDetailModel, VideoGameEntityMapper>(
        unitOfWorkFactory, modelMapper), IVideoGameFacade
{

}