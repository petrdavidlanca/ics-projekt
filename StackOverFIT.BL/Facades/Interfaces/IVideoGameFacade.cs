using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;

namespace StackOverFIT.BL.Facades;

public interface IVideoGameFacade : IFacade<VideoGameEntity, VideoGameListModel, VideoGameDetailModel>
{
    Task<IEnumerable<VideoGameListModel>> GetFilteredGamesAsync(string? searchTerm, string? sortBy, bool ascending = true, int? maxPegiRating = null);
    Task<IEnumerable<VideoGameListModel>> GetGamesByLibraryIdAsync(Guid libraryId);
}