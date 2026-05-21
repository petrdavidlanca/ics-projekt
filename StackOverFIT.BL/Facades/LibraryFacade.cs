using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Mappers;
using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
using StackOverFIT.DAL.Mappers;
using StackOverFIT.DAL.Repositories;
using StackOverFIT.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace StackOverFIT.BL.Facades;

public class LibraryFacade(
    IUnitOfWorkFactory unitOfWorkFactory,
    LibraryModelMapper modelMapper)
    : FacadeBase<LibraryEntity, LibraryListModel, LibraryDetailModel, LibraryEntityMapper>(
        unitOfWorkFactory, modelMapper), ILibraryFacade
{
    protected override ICollection<string> IncludesNavigationPathDetail =>
        new[] { nameof(LibraryEntity.VideoGames) };

    public async Task AddVideoGameAsync(Guid libraryId, Guid videoGameId)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        var junctionRepo = uow.GetRepository<LibraryVideoGameEntity, LibraryVideoGameEntityMapper>();

        bool exists = await junctionRepo.Get()
            .AnyAsync(x => x.LibraryId == libraryId && x.VideoGameId == videoGameId);

        if (!exists)
        {
            junctionRepo.Insert(new LibraryVideoGameEntity
            {
                Id = Guid.NewGuid(),
                LibraryId = libraryId,
                VideoGameId = videoGameId
            });
            await uow.CommitAsync();
        }
    }

    public async Task RemoveVideoGameAsync(Guid libraryId, Guid videoGameId)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        var junctionRepo = uow.GetRepository<LibraryVideoGameEntity, LibraryVideoGameEntityMapper>();

        var junction = await junctionRepo.Get()
            .SingleOrDefaultAsync(x => x.LibraryId == libraryId && x.VideoGameId == videoGameId);

        if (junction != null)
        {
            await junctionRepo.DeleteAsync(junction.Id);
            await uow.CommitAsync();
        }
    }

    public async Task<IEnumerable<LibraryListModel>> GetFilteredLibrariesAsync(string? searchTerm, string? sortBy, bool ascending = true)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        IQueryable<LibraryEntity> query = uow.GetRepository<LibraryEntity, LibraryEntityMapper>().Get().Include(l => l.VideoGames);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(l => l.Name.ToLower().Contains(searchTerm));
        }

        query = sortBy?.ToLower() switch
        {
            "name" => ascending ? query.OrderBy(l => l.Name) : query.OrderByDescending(l => l.Name),
            _ => query.OrderBy(l => l.Name)
        };

        var entities = await query.ToListAsync().ConfigureAwait(false);
        return ModelMapper.MapToListModel(entities);
    }
}