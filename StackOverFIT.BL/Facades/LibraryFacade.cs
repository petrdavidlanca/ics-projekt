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
    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[] {nameof(LibraryEntity.VideoGames)};

    public async Task AddVideoGameAsync(Guid libraryId, Guid videoGameId)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        IRepository<VideoGameEntity> gameRepository =
            uow.GetRepository<VideoGameEntity, VideoGameEntityMapper>();

        VideoGameEntity? game = await gameRepository.GetByIdAsync(videoGameId);

        if (game != null)
        {
            game.LibraryId = libraryId;

            await gameRepository.UpdateAsync(game);
            await uow.CommitAsync();
        }
    }

    public async Task RemoveVideoGameAsync(Guid libraryId, Guid videoGameId)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        IRepository<VideoGameEntity> gameRepository =
            uow.GetRepository<VideoGameEntity, VideoGameEntityMapper>();

        VideoGameEntity? game = await gameRepository.GetByIdAsync(videoGameId);

        if (game != null && game.LibraryId == libraryId)
        {
            game.LibraryId = null;

            await gameRepository.UpdateAsync(game);
            await uow.CommitAsync();
        }
    }
}