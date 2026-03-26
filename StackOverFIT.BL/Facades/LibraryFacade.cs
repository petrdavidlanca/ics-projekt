using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Mappers;
using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
using StackOverFIT.DAL.Mappers;
using StackOverFIT.DAL.Repositories;
using StackOverFIT.DAL.UnitOfWork;

namespace StackOverFIT.BL.Facades;

public class LibraryFacade(
    IUnitOfWorkFactory unitOfWorkFactory,
    LibraryModelMapper modelMapper)
    : FacadeBase<LibraryEntity, LibraryListModel, LibraryDetailModel, LibraryEntityMapper>(
        unitOfWorkFactory, modelMapper), ILibraryFacade
{
    public async Task AddVideoGameAsync(Guid libraryId, Guid videoGameId) { }

    public async Task RemoveVideoGameAsync(Guid libraryId, Guid videoGameId) { }
}