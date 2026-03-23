using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
using StackOverFIT.BL.Facades;

namespace StackOverFIT.BL.Facades;

public interface IVideoGameFacade : IFacade<video_game_entity, VideoGameListModel, VideoGameDetailModel>
{
}
