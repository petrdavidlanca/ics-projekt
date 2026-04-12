using System;
using System.Linq;
using System.Threading.Tasks;
using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
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
        // Arrange
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

        // Act
        await _libraryFacade.AddVideoGameAsync(library.Id, game.Id);

        // Assert
        var updatedGame = await StackOverFITDbContextSUT.VideoGames.FindAsync(game.Id);

        Assert.NotNull(updatedGame);
        Assert.Equal(library.Id, updatedGame!.LibraryId);
    }
    
}
