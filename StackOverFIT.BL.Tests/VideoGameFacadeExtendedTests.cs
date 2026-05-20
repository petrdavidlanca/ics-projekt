using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StackOverFIT.BL.Facades;
using StackOverFIT.BL.Mappers;
using StackOverFIT.BL.Models;
using StackOverFIT.DAL.Entities;
using StackOverFIT.DAL.Mappers;
using Xunit;

namespace StackOverFIT.BL.Tests;

public class VideoGameFacadeExtendedTests : FacadeTestsBase
{
    private readonly VideoGameFacade _facade;

    public VideoGameFacadeExtendedTests()
        : base(null!)
    {
        _facade = new VideoGameFacade(UnitOfWorkFactory, new VideoGameModelMapper());
    }

    private async Task SeedGamesAsync()
    {
        StackOverFITDbContextSUT.VideoGames.AddRange(
            new VideoGameEntity
            {
                Id = Guid.NewGuid(),
                Name = "Cyberpunk",
                Description = "Futuristic RPG",
                PegiRating = 18,
                CreatedAt = new DateTime(2020, 12, 10)
            },
            new VideoGameEntity
            {
                Id = Guid.NewGuid(),
                Name = "Witcher 3",
                Description = "Fantasy RPG",
                PegiRating = 16,
                CreatedAt = new DateTime(2015, 5, 19)
            },
            new VideoGameEntity
            {
                Id = Guid.NewGuid(),
                Name = "Minecraft",
                Description = "Creative sandbox",
                PegiRating = 7,
                CreatedAt = new DateTime(2011, 11, 18)
            }
        );

        await StackOverFITDbContextSUT.SaveChangesAsync();
    }

    [Fact]
    public async Task GetFilteredGamesAsync_Filters_By_SearchTerm()
    {
        await SeedGamesAsync();

        var result = await _facade.GetFilteredGamesAsync("rpg", null);

        Assert.Equal(2, result.Count()); // Cyberpunk + Witcher
    }

    [Fact]
    public async Task GetFilteredGamesAsync_SearchTerm_Ignores_Case()
    {
        await SeedGamesAsync();

        var result = await _facade.GetFilteredGamesAsync("MINE", null);

        Assert.Single(result);
        Assert.Equal("Minecraft", result.First().Name);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_Filters_By_PegiRating()
    {
        await SeedGamesAsync();

        var result = await _facade.GetFilteredGamesAsync(null, null, true, maxPegiRating: 12);

        Assert.Single(result);
        Assert.Equal("Minecraft", result.First().Name);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_Sorts_By_Name_Ascending()
    {
        await SeedGamesAsync();

        var result = await _facade.GetFilteredGamesAsync(null, "name", true);

        Assert.Equal(new[] { "Cyberpunk", "Minecraft", "Witcher 3" },
            result.Select(r => r.Name).ToArray());
    }

    [Fact]
    public async Task GetFilteredGamesAsync_Sorts_By_Date_Ascending()
    {
        await SeedGamesAsync();

        var result = await _facade.GetFilteredGamesAsync(null, "date", true);

        Assert.Equal(new[] { "Minecraft", "Witcher 3", "Cyberpunk" },
            result.Select(r => r.Name).ToArray());
    }

    [Fact]
    public async Task GetFilteredGamesAsync_Combines_Search_And_Pegi()
    {
        await SeedGamesAsync();

        var result = await _facade.GetFilteredGamesAsync("rpg", null, true, maxPegiRating: 16);

        Assert.Single(result);
        Assert.Equal("Witcher 3", result.First().Name);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_Returns_Empty_When_No_Match()
    {
        await SeedGamesAsync();

        var result = await _facade.GetFilteredGamesAsync("nonexistent", null);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetFilteredGamesAsync_Allows_Null_Parameters()
    {
        await SeedGamesAsync();

        var result = await _facade.GetFilteredGamesAsync(null, null);

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetGamesByLibraryIdAsync_Returns_Only_Games_In_Library()
    {
        var library1 = new LibraryEntity { Id = Guid.NewGuid(), Name = "Lib1" };
        var library2 = new LibraryEntity { Id = Guid.NewGuid(), Name = "Lib2" };

        var game1 = new VideoGameEntity
        {
            Id = Guid.NewGuid(),
            Name = "Game1",
            PegiRating = 12,
            Libraries = { library1 },
            Description = "Help"
        };

        var game2 = new VideoGameEntity
        {
            Id = Guid.NewGuid(),
            Name = "Game2",
            PegiRating = 16,
            Libraries = { library2 },
            Description = "Help"
        };

        StackOverFITDbContextSUT.Libraries.AddRange(library1, library2);
        StackOverFITDbContextSUT.VideoGames.AddRange(game1, game2);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        var result = await _facade.GetGamesByLibraryIdAsync(library1.Id);

        Assert.Single(result);
        Assert.Equal("Game1", result.First().Name);
    }

    [Fact]
    public async Task GetGamesByLibraryIdAsync_Returns_Empty_When_No_Games()
    {
        var library = new LibraryEntity { Id = Guid.NewGuid(), Name = "EmptyLib" };

        StackOverFITDbContextSUT.Libraries.Add(library);
        await StackOverFITDbContextSUT.SaveChangesAsync();

        var result = await _facade.GetGamesByLibraryIdAsync(library.Id);

        Assert.Empty(result);
    }
}
