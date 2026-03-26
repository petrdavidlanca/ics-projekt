using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
using StackOverFIT.BL.Facades;

namespace StackOverFIT.BL.Facades;

public interface ILibraryFacade : IFacade<library_entity, LibraryListModel, LibraryDetailModel>
{
    Task AddVideoGameAsync(Guid libraryId, Guid videoGameId);
    Task RemoveVideoGameAsync(Guid libraryId, Guid videoGameId);
}
