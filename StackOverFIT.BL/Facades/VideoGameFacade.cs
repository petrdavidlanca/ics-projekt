using StackOverFIT.BL.Mappers;
using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
using StackOverFIT.DAL.Mappers;
using StackOverFIT.DAL.Repositories;
using StackOverFIT.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace StackOverFIT.BL.Facades;

public class VideoGameFacade(
    IUnitOfWorkFactory unitOfWorkFactory,
    VideoGameModelMapper modelMapper)
    : FacadeBase<VideoGameEntity, VideoGameListModel, VideoGameDetailModel, VideoGameEntityMapper>(
        unitOfWorkFactory, modelMapper), IVideoGameFacade
{
    public async Task<IEnumerable<VideoGameListModel>> GetFilteredGamesAsync(string? searchTerm, string? sortBy, bool ascending = true, int? maxPegiRating = null)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        IQueryable<VideoGameEntity> query = uow.GetRepository<VideoGameEntity, VideoGameEntityMapper>().Get();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(g => g.Name.ToLower().Contains(searchTerm) ||
                                     (g.Description != null && g.Description.ToLower().Contains(searchTerm)));
        }

        if (maxPegiRating.HasValue)
        {
            query = query.Where(g => g.PegiRating <= maxPegiRating.Value);
        }

        query = sortBy?.ToLower() switch
        {
            "name" => ascending ? query.OrderBy(g => g.Name) : query.OrderByDescending(g => g.Name),
            "rating" => ascending ? query.OrderBy(g => g.PegiRating) : query.OrderByDescending(g => g.PegiRating),
            _ => query.OrderBy(g => g.Name)
        };

        var entities = await query.ToListAsync().ConfigureAwait(false);
        return ModelMapper.MapToListModel(entities);
    }

    public async Task<IEnumerable<VideoGameListModel>> GetGamesByLibraryIdAsync(Guid libraryId)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        IQueryable<VideoGameEntity> query = uow.GetRepository<VideoGameEntity, VideoGameEntityMapper>().Get()
            .Where(g => g.Libraries.Any(l => l.Id == libraryId));

        var entities = await query.ToListAsync().ConfigureAwait(false);
        return ModelMapper.MapToListModel(entities);
    }
}