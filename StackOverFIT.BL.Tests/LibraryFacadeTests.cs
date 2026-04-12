using Microsoft.VisualStudio.TestPlatform.Utilities;
using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace StackOverFIT.BL.Tests;

public class LibraryFacadeTests : FacadeTestsBase
{
    private readonly LibraryFacade _libraryFacade;

    public LibraryFacadeTests(ITestOutputHelper output)
        : base(output)
    {
        _libraryFacade = new LibraryFacade(UnitOfWorkFactory, LibraryModelMapper);
    }

    [Fact]
    public async Task AddVideoGameAsync_Adds_Game_To_Library()
    {
        var library = new LibraryEntity
        {
            Id = Guid.NewGuid(),
            Name = "My Library"
        };

        var game = new VideoGameEntity
        {
            Id = Guid.NewGuid(),
            Name = "Witcher 3",
            Description = "RPG",
            PegiRating = 18
        };

        StackOverFITDbContextSUT.Libraries.Add(library);
        StackOverFITDbContextSUT.VideoGames.Add(game);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        await _libraryFacade.AddVideoGameAsync(library.Id, game.Id);

        await using var verifyCtx = DbContextFactory.CreateDbContext();
        var updatedGame = await verifyCtx.VideoGames.FindAsync(game.Id);

        Assert.NotNull(updatedGame);
        Assert.Equal(library.Id, updatedGame!.LibraryId);
    }

    [Fact]
    public async Task RemoveVideoGameAsync_Removes_Game_From_Library()
    {
        var library = new LibraryEntity
        {
            Id = Guid.NewGuid(),
            Name = "My Library"
        };

        var game = new VideoGameEntity
        {
            Id = Guid.NewGuid(),
            Name = "Cyberpunk 2077",
            Description = "Sci-fi RPG",
            PegiRating = 18,
            LibraryId = library.Id
        };

        StackOverFITDbContextSUT.Libraries.Add(library);
        StackOverFITDbContextSUT.VideoGames.Add(game);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        await _libraryFacade.RemoveVideoGameAsync(library.Id, game.Id);

        await using var verifyCtx = DbContextFactory.CreateDbContext();
        var updatedGame = await verifyCtx.VideoGames.FindAsync(game.Id);

        Assert.NotNull(updatedGame);
        Assert.Null(updatedGame!.LibraryId);
    }

    [Fact]
    public async Task SaveAsync_Then_GetAsync_Returns_Saved_Library()
    {
        var model = new LibraryDetailModel
        {
            Id = Guid.Empty,
            Name = "My Library"
        };

        var saved = await _libraryFacade.SaveAsync(model);
        var loaded = await _libraryFacade.GetAsync(saved.Id);

        Assert.NotNull(loaded);
        Assert.Equal("My Library", loaded!.Name);
    }

    [Fact]
    public async Task SaveAsync_Creates_New_Library()
    {
        var model = new LibraryDetailModel
        {
            Id = Guid.Empty,
            Name = "New Library"
        };

        var saved = await _libraryFacade.SaveAsync(model);

        Assert.NotEqual(Guid.Empty, saved.Id);

        await using var verifyCtx = DbContextFactory.CreateDbContext();
        var entity = await verifyCtx.Libraries.FindAsync(saved.Id);

        Assert.NotNull(entity);
        Assert.Equal("New Library", entity!.Name);
    }

    [Fact]
    public async Task SaveAsync_Updates_Existing_Library()
    {
        var library = new LibraryEntity
        {
            Id = Guid.NewGuid(),
            Name = "Original"
        };

        StackOverFITDbContextSUT.Libraries.Add(library);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        var model = new LibraryDetailModel
        {
            Id = library.Id,
            Name = "Updated"
        };

        var updated = await _libraryFacade.SaveAsync(model);

        Assert.Equal("Updated", updated.Name);

        await using var verifyCtx = DbContextFactory.CreateDbContext();
        var entity = await verifyCtx.Libraries.FindAsync(library.Id);

        Assert.Equal("Updated", entity!.Name);
    }

    [Fact]
    public async Task GetAsync_ReturnsNull_When_Library_Not_Found()
    {
        var result = await _libraryFacade.GetAsync(Guid.NewGuid());
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsync_Returns_All_Libraries()
    {
        await _libraryFacade.SaveAsync(new LibraryDetailModel { Name = "Lib1" });
        await _libraryFacade.SaveAsync(new LibraryDetailModel { Name = "Lib2" });

        var all = await _libraryFacade.GetAsync();

        Assert.True(all.Count() >= 2);
    }

    [Fact]
    public async Task DeleteAsync_Removes_Library()
    {
        var library = await _libraryFacade.SaveAsync(new LibraryDetailModel { Name = "ToDelete" });

        await _libraryFacade.DeleteAsync(library.Id);

        await using var verifyCtx = DbContextFactory.CreateDbContext();
        var entity = await verifyCtx.Libraries.FindAsync(library.Id);

        Assert.Null(entity);
    }

    [Fact]
    public async Task DeleteAsync_DoesNotThrow_When_Library_Not_Found()
    {
        await _libraryFacade.DeleteAsync(Guid.NewGuid());
    }

    [Fact]
    public async Task AddVideoGameAsync_DoesNothing_When_Game_Already_Assigned()
    {
        var library = await _libraryFacade.SaveAsync(new LibraryDetailModel { Name = "Lib" });

        var game = new VideoGameEntity
        {
            Id = Guid.NewGuid(),
            Name = "Game",
            Description = "Desc",
            PegiRating = 12,
            LibraryId = library.Id
        };

        StackOverFITDbContextSUT.VideoGames.Add(game);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        await _libraryFacade.AddVideoGameAsync(library.Id, game.Id);

        await using var verifyCtx = DbContextFactory.CreateDbContext();
        var updated = await verifyCtx.VideoGames.FindAsync(game.Id);

        Assert.Equal(library.Id, updated!.LibraryId);
    }

    [Fact]
    public async Task RemoveVideoGameAsync_DoesNothing_When_Game_Already_Unassigned()
    {
        var library = await _libraryFacade.SaveAsync(new LibraryDetailModel { Name = "Lib" });

        var game = new VideoGameEntity
        {
            Id = Guid.NewGuid(),
            Name = "Game",
            Description = "Desc",
            PegiRating = 12,
            LibraryId = null
        };

        StackOverFITDbContextSUT.VideoGames.Add(game);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        await _libraryFacade.RemoveVideoGameAsync(library.Id, game.Id);

        await using var verifyCtx = DbContextFactory.CreateDbContext();
        var updated = await verifyCtx.VideoGames.FindAsync(game.Id);

        Assert.Null(updated!.LibraryId);
    }

}
