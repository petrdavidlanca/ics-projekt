using Microsoft.EntityFrameworkCore;
using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Mappers;
using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
using StackOverFIT.DAL.Enums;
using System;
using System.Threading.Tasks;
using Xunit;

namespace StackOverFIT.BL.Tests;

public class VideoGameFacadeTests : FacadeTestsBase
{
    private readonly VideoGameFacade _videoGameFacade;

    public VideoGameFacadeTests()
        : base(null!)
    {
        _videoGameFacade = new VideoGameFacade(UnitOfWorkFactory, new VideoGameModelMapper());
    }

    [Fact]
    public async Task SaveAsync_Creates_New_VideoGame()
    {
        var model = new VideoGameDetailModel
        {
            Id = Guid.Empty,
            Name = "Cyberpunk 2077",
            Description = "Sci-fi RPG",
            PegiRating = 18
        };

        var saved = await _videoGameFacade.SaveAsync(model);

        Assert.NotEqual(Guid.Empty, saved.Id);

        await using var verifyCtx = DbContextFactory.CreateDbContext();
        var entity = await verifyCtx.VideoGames.FindAsync(saved.Id);

        Assert.NotNull(entity);
        Assert.Equal("Cyberpunk 2077", entity!.Name);
    }

    [Fact]
    public async Task SaveAsync_Updates_Existing_VideoGame()
    {
        var game = new VideoGameEntity
        {
            Id = Guid.NewGuid(),
            Name = "Original",
            Description = "Desc",
            PegiRating = 12
        };

        StackOverFITDbContextSUT.VideoGames.Add(game);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        var model = new VideoGameDetailModel
        {
            Id = game.Id,
            Name = "Updated",
            Description = "New Desc",
            PegiRating = 18
        };

        var updated = await _videoGameFacade.SaveAsync(model);

        Assert.Equal("Updated", updated.Name);

        await using var verifyCtx = DbContextFactory.CreateDbContext();
        var entity = await verifyCtx.VideoGames.FindAsync(game.Id);

        Assert.Equal("Updated", entity!.Name);
        Assert.Equal("New Desc", entity.Description);
        Assert.Equal(18, entity.PegiRating);
    }

    [Fact]
    public async Task GetAsync_Returns_VideoGameDetail()
    {
        var game = new VideoGameEntity
        {
            Id = Guid.NewGuid(),
            Name = "Witcher 3",
            Description = "RPG",
            PegiRating = 18
        };

        StackOverFITDbContextSUT.VideoGames.Add(game);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        var loaded = await _videoGameFacade.GetAsync(game.Id);

        Assert.NotNull(loaded);
        Assert.Equal("Witcher 3", loaded!.Name);
    }

    [Fact]
    public async Task GetAsync_ReturnsNull_When_NotFound()
    {
        var result = await _videoGameFacade.GetAsync(Guid.NewGuid());
        Assert.Null(result);
    }
    [Fact]
    public async Task GetAsync_Returns_All_VideoGames()
    {
        StackOverFITDbContextSUT.VideoGames.Add(new VideoGameEntity
        {
            Id = Guid.NewGuid(),
            Name = "Game1",
            Description = "Desc1",
            PegiRating = 12
        });

        StackOverFITDbContextSUT.VideoGames.Add(new VideoGameEntity
        {
            Id = Guid.NewGuid(),
            Name = "Game2",
            Description = "Desc2",
            PegiRating = 16
        });

        await StackOverFITDbContextSUT.SaveChangesAsync();

        var all = await _videoGameFacade.GetAsync();

        Assert.True(all.Count() >= 2);
    }
    [Fact]
    public async Task DeleteAsync_Removes_VideoGame()
    {
        var game = new VideoGameEntity
        {
            Id = Guid.NewGuid(),
            Name = "ToDelete",
            Description = "Desc",
            PegiRating = 12
        };

        StackOverFITDbContextSUT.VideoGames.Add(game);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        await _videoGameFacade.DeleteAsync(game.Id);

        await using var verifyCtx = DbContextFactory.CreateDbContext();
        var entity = await verifyCtx.VideoGames.FindAsync(game.Id);

        Assert.Null(entity);
    }

    [Fact]
    public async Task DeleteAsync_DoesNotThrow_When_NotFound()
    {
        await _videoGameFacade.DeleteAsync(Guid.NewGuid());
    }

}
